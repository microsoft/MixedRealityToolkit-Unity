#include <pch.h>
#include <assert.h>
#include <math.h>
#include <vector>
#include <deque>
#include <unordered_map>
#include <memory>

#include <d3d11_2.h>
#include "Unity/IUnityGraphics.h"
#include "Unity/IUnityGraphicsD3D11.h"

using namespace std;

class Query;
typedef shared_ptr<Query> QueryPtr;
typedef shared_ptr<deque<QueryPtr>> QueryDequePtr;
typedef shared_ptr<CRITICAL_SECTION> CriticalSectionPtr;

static UnityGfxRenderer s_DeviceType = kUnityGfxRendererNull;
static IUnityInterfaces* s_UnityInterfaces = NULL;
static IUnityGraphics* s_Graphics = NULL;

static ID3D11Device* s_Device = NULL;
static ID3D11DeviceContext* s_Context = NULL;

static ID3DUserDefinedAnnotation* s_Annotation = NULL;

static const int MaxActiveQueries = 5;

static unordered_map<int, QueryDequePtr> s_ActiveQueries;
static vector<QueryPtr> s_QueryPool;
static unordered_map<int, double> s_FrameTimes;

static CRITICAL_SECTION updateSync;

enum QueryState
{
	Unknown,
	BeginSubmitted,
	EndSubmitted,
};

class Query
{
private:
	ID3D11Query* mDisjoint = nullptr;
	ID3D11Query* mBegin = nullptr;
	ID3D11Query* mEnd = nullptr;

public:

	QueryState State = QueryState::Unknown;

	Query() 
	{ 
	}

	~Query()
	{
		DestroyEventQueries();
	}

	bool Begin()
	{
		if (mDisjoint == nullptr)
		{
			CreateEventQueries();
		}

		s_Context->Begin(mDisjoint);
		s_Context->End(mBegin);
		s_Annotation->SetMarker(L"OnRenderEvent-BeginFrame");

		State = QueryState::BeginSubmitted;
		return true;
	}

	bool End()
	{
		if (State != QueryState::BeginSubmitted)
		{
			return false;
			s_Annotation->SetMarker(L"OnRenderEvent-EndFrame(Failed)");
		}

		s_Context->End(mEnd);
		s_Context->End(mDisjoint);
		s_Annotation->SetMarker(L"OnRenderEvent-EndFrame");

		State = QueryState::EndSubmitted;
		return true;
	}

	bool Read(double* value)
	{
		if (State != QueryState::EndSubmitted)
		{
			return false;
		}

		D3D11_QUERY_DATA_TIMESTAMP_DISJOINT disjointData;
		if (s_Context->GetData(mDisjoint, &disjointData, sizeof(disjointData), D3D11_ASYNC_GETDATA_DONOTFLUSH) == S_OK)
		{
			if (!disjointData.Disjoint)
			{
				UINT64 frameTimeBegin;
				if (s_Context->GetData(mBegin, &frameTimeBegin, sizeof(frameTimeBegin), D3D11_ASYNC_GETDATA_DONOTFLUSH) == S_OK)
				{
					UINT64 frameTimeEnd;

					if (s_Context->GetData(mEnd, &frameTimeEnd, sizeof(frameTimeEnd), D3D11_ASYNC_GETDATA_DONOTFLUSH) == S_OK)
					{
						double frameTime = (double)(frameTimeEnd - frameTimeBegin) / (double)disjointData.Frequency;
						*value = frameTime;

						State = QueryState::Unknown;
						return true;
					}
				}
			}
			else
			{
				State = QueryState::Unknown;
				*value = -1;
				return true;
			}
		}

		return false;
	}

	void CreateEventQueries()
	{
		D3D11_QUERY_DESC disjointDesc;
		disjointDesc.Query = D3D11_QUERY_TIMESTAMP_DISJOINT;
		disjointDesc.MiscFlags = 0;

		D3D11_QUERY_DESC desc;
		desc.Query = ::D3D11_QUERY_TIMESTAMP;
		desc.MiscFlags = 0;

		s_Device->CreateQuery(&disjointDesc, &mDisjoint);
		s_Device->CreateQuery(&desc, &mBegin);
		s_Device->CreateQuery(&desc, &mEnd);
	}

	void DestroyEventQueries()
	{
		if (mDisjoint == nullptr)
		{
			mDisjoint->Release();
			mBegin->Release();
			mEnd->Release();
		}
	}
};

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType);

static void ReleaseResources();

typedef enum RenderEventType
{
	kRenderEventTypeBeginFrameEventMin = 1000,
	kRenderEventTypeBeginFrameEventMax = 1999,
	kRenderEventTypeEndFrameEventMin = 2000,
	kRenderEventTypeEndFrameEventMax = 2999,
} RenderEventType;

extern "C" void	UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	s_UnityInterfaces = unityInterfaces;
	s_Graphics = s_UnityInterfaces->Get<IUnityGraphics>();
	s_Graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);

	InitializeCriticalSection(&updateSync);

	// Run OnGraphicsDeviceEvent(initialize) manually on plugin load
	// to not miss the event in case the graphics device is already initialized.
	OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
	if (s_Graphics != NULL)
	{
		ReleaseResources();
		s_Graphics->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
	}
}

extern "C" double UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetLastFrameGPUTime(int eventId)
{
	if (s_Context == NULL)
	{
		return 0;
	}

	double time = 0;

	EnterCriticalSection(&updateSync);
	auto it = s_FrameTimes.find(eventId);
	if (it != s_FrameTimes.end())
	{
		time = it->second;
	}
	LeaveCriticalSection(&updateSync);

	return time;
}

static void UpdateFrameTime()
{
	unordered_map<int, QueryDequePtr>::iterator activeQueriesIt;

	for (activeQueriesIt = s_ActiveQueries.begin(); activeQueriesIt != s_ActiveQueries.end(); activeQueriesIt++)
	{
		if (activeQueriesIt->second->empty())
		{
			continue;
		}

		auto activeQueries = activeQueriesIt->second;

		double frameTime = 0;
		QueryPtr q = activeQueries->front();

		if (q->Read(&frameTime))
		{
			activeQueries->pop_front();
			s_QueryPool.push_back(q);

			EnterCriticalSection(&updateSync);
			s_FrameTimes[activeQueriesIt->first] = frameTime;
			LeaveCriticalSection(&updateSync);
		}
	}
}

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
{
	// Create graphics API implementation upon initialization
	if (eventType == kUnityGfxDeviceEventInitialize)
	{
		s_DeviceType = s_Graphics->GetRenderer();

		if (s_DeviceType == kUnityGfxRendererD3D11)
		{
			// D3D11 is the only API supported.

			IUnityGraphicsD3D11* d3d = s_UnityInterfaces->Get<IUnityGraphicsD3D11>();
			s_Device = d3d->GetDevice();
			s_Device->GetImmediateContext(&s_Context);

			// Annotations allow events to be logged so that they are visible in GPUView when profiling.
			s_Context->QueryInterface(__uuidof(s_Annotation), reinterpret_cast<void**>(&s_Annotation));
		}
	}

	if (eventType == kUnityGfxDeviceEventShutdown)
	{
		ReleaseResources();
		s_DeviceType = kUnityGfxRendererNull;
	}
}

static void UNITY_INTERFACE_API OnRenderEvent(int eventID)
{
	// Unknown / unsupported graphics device type? Do nothing
	if (s_Context == NULL)
	{
		return;
	}

	// Retrieve all available events.
	UpdateFrameTime();

	if (eventID >= kRenderEventTypeBeginFrameEventMin && eventID <= kRenderEventTypeBeginFrameEventMax)
	{
		int eventId = eventID - kRenderEventTypeBeginFrameEventMin;
		QueryDequePtr activeQueries = nullptr;

		auto activeQueriesIt = s_ActiveQueries.find(eventId);
		if (activeQueriesIt != s_ActiveQueries.end())
		{
			activeQueries = activeQueriesIt->second;
		}
		else
		{
			QueryDequePtr newActiveQueries = make_shared<deque<QueryPtr>>();
			s_ActiveQueries.emplace(eventId, newActiveQueries);
			activeQueries = newActiveQueries;
		}

		if (s_QueryPool.empty())
		{
			if (activeQueries->size() > MaxActiveQueries)
			{
				return;
			}
			else
			{
				s_QueryPool.push_back(make_shared<Query>());
			}
		}

		QueryPtr q = s_QueryPool.back();
		s_QueryPool.pop_back();

		q->Begin();
		activeQueries->push_back(q);
	}
	else if (eventID >= kRenderEventTypeEndFrameEventMin && eventID <= kRenderEventTypeEndFrameEventMax)
	{
		int eventId = eventID - kRenderEventTypeEndFrameEventMin;
		auto activeQueriesIt = s_ActiveQueries.find(eventId);

		if (activeQueriesIt != s_ActiveQueries.end())
		{
			activeQueriesIt->second->back()->End();
		}
	}
}

static void ReleaseResources()
{
	if (s_Annotation != NULL)
	{
		s_Annotation->Release();
		s_Annotation = NULL;
	}

	s_ActiveQueries.clear();
	s_QueryPool.clear();

	DeleteCriticalSection(&updateSync);
}

// GetRenderEventFunc, an example function we export which is used to get a rendering event callback function.
extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc()
{
	return OnRenderEvent;
}