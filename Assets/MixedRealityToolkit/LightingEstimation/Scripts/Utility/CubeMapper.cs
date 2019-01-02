using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMapper {
	#region Support data
	struct StampDir {
		public Quaternion direction;
		public Vector3    position;
		public float      timestamp;
	}

	const int   cSphereRows = 12;
	const int   cSphereColumns = 18;
	const float cSphereSize = 10;
	const int   cGeometryLayer = 30;
	#endregion

	#region Fields
	GameObject _root;
	Camera     _cam;
	Cubemap    _map;
	Renderer   _imageQuad;
	Renderer   _imageSphere;
	Material   _skybox;
	float      _fov;
	bool       _hdr;
	
	bool           _needsFullStamp      = true;
	float          _stampExpireDistance = 0;
	float          _stampExpireLife     = 0;
	List<StampDir> _stampCache          = new List<StampDir>();
	#endregion

	#region Properties
	public Cubemap  Map         { get { return _map; } }
	public Material SkyMaterial { get { return _skybox; } }
	public float    StampExpireDistance { get { return _stampExpireDistance; } set { _stampExpireDistance = value; } }
	#endregion

	#region Public Methods
	public void Create(float aStampFOVDeg, int aResolution=128, bool aHDR=false) {
		_fov = aStampFOVDeg;
		_hdr = aHDR;
		
		_map = new Cubemap(aResolution, _hdr ? TextureFormat.RGBAHalf : TextureFormat.RGB24, true);

		_skybox = new Material(Shader.Find("Skybox/Cubemap"));
		_skybox.SetTexture("_Tex", _map);

		// Create the root object
		_root = new GameObject("_Cubemap Generator Root");
		_root.SetActive(false);
		// This throws errors when destroying objects at runtime
		//_root.hideFlags = HideFlags.DontSave;

		// Create a camera to render with
		GameObject camObj = new GameObject("RenderCamera");
		_cam = camObj.AddComponent<Camera>();
		_cam.cullingMask     = 1 << cGeometryLayer;
		_cam.clearFlags      = CameraClearFlags.Skybox;
		_cam.backgroundColor = new Color(0,0,0,1);
		camObj.transform.SetParent(_root.transform);

		// Add a quad for stamping camera pictures with
		GameObject quad = new GameObject("StampQuad");
		_imageQuad = quad.AddComponent<MeshRenderer>();
		_imageQuad.sharedMaterial           = new Material(Shader.Find("Unlit/VertTransparent"));
		_imageQuad.sharedMaterial.hideFlags = HideFlags.DontSave;
		quad.AddComponent<MeshFilter>().sharedMesh = CreatePlane(1f/Mathf.Tan(_fov*Mathf.Deg2Rad*0.5f));
		quad.layer = cGeometryLayer;
		quad.transform.SetParent(_root.transform);

		// Add a nautilus sphere for initial totally surrounding stamps
		GameObject sphere = new GameObject("StampSphere");
		_imageSphere = sphere.AddComponent<MeshRenderer>();
		_imageSphere.sharedMaterial = Object.Instantiate(_imageQuad.sharedMaterial);
		_imageSphere.sharedMaterial.renderQueue-=1;
		_imageSphere.hideFlags      = HideFlags.DontSave;
		sphere.AddComponent<MeshFilter>().sharedMesh = CreateNautilusSphere();
		sphere.layer = cGeometryLayer;
		sphere.transform.SetParent(_root.transform);
	}
	public void Destroy() {
		if (_root == null)
			return;
		
		if (Application.isPlaying)
			Object.Destroy(_root);
		else
			Object.DestroyImmediate(_root);
	}
	public void Stamp(Texture aTex, Vector3 aPosition, Quaternion aOrientation) {
		if (_imageQuad == null) {
			Debug.LogError("[LightingEstimation] Please call CubeMapper.Create before attempting to Stamp anything!");
			return;
		}
		
		// Prep objects for rendering
		if (_needsFullStamp) {
			_imageQuad.enabled = true;
			_imageSphere.enabled = true;
			_imageSphere.transform.rotation = aOrientation;
			_imageSphere.sharedMaterial.mainTexture = aTex;
		} else {
			_imageQuad.enabled = true;
			_imageSphere.enabled = false;
		}
		_cam.transform.rotation = aOrientation;
		_imageQuad.transform.rotation = aOrientation;
		_imageQuad.sharedMaterial.mainTexture = aTex;
		if (aTex != null)
			_imageQuad.transform.localScale = new Vector3(1, (aTex.height / (float)aTex.width), 1) ;
		
		// Figure out which faces of the Cubemap we need to render with this stamp, we don't generally need to 
		// render all of them, (usually 3 at most) so it's a no-brainer optimization, even if it's not 100% accurate
		int     render = 0;
		Vector3 fwd    = _cam.transform.forward;
		if (_needsFullStamp) {
			render = 63;
		} else {
			if (AngleVisible(fwd,  Vector3.right  )) render |= 1 << (int)CubemapFace.PositiveX;
			if (AngleVisible(fwd, -Vector3.right  )) render |= 1 << (int)CubemapFace.NegativeX;
			if (AngleVisible(fwd,  Vector3.up     )) render |= 1 << (int)CubemapFace.PositiveY;
			if (AngleVisible(fwd, -Vector3.up     )) render |= 1 << (int)CubemapFace.NegativeY;
			if (AngleVisible(fwd,  Vector3.forward)) render |= 1 << (int)CubemapFace.PositiveZ;
			if (AngleVisible(fwd, -Vector3.forward)) render |= 1 << (int)CubemapFace.NegativeZ;
		}

		// render the objects
		_root.SetActive(true);
		Material old = RenderSettings.skybox;
		RenderSettings.skybox = _skybox;
		_cam.RenderToCubemap(_map, render);
		RenderSettings.skybox = old;
		_root.SetActive(false);
		
		CacheStamp(aPosition, aOrientation);
		_needsFullStamp = false;
	}
	public void Clear() {
		_needsFullStamp = true;
		ClearCache();
	}
	public void ClearCache() {
		_stampCache.Clear();
	}
	public bool IsCached(Vector3 aPosition, Quaternion aOrientation) {
		float min = float.MaxValue;

		for (int i = 0; i < _stampCache.Count; i++) {
			StampDir s = _stampCache[i];
			if  (IsExpired(i, aPosition)) {
				_stampCache.RemoveAt(i);
				continue;
			}
			float ang = Quaternion.Angle(aOrientation, s.direction);
			if (min > ang)
				min = ang;
		}

		// allow for a little overlap (using .4 fov instead of .5), especially since fov is horiontal, not vertical, and vertical is 
		// shorter due to aspect ratio
		return min < _fov*(9f/16f);
	}
	#endregion

	#region Private Methods
	private Mesh CreatePlane(float aZ, float aBlend = 0.2f) {
		Mesh m = new Mesh();
		m.vertices = new Vector3[] { 
			new Vector3(-1,1,aZ), new Vector3(1,1,aZ), new Vector3(1,-1,aZ), new Vector3(-1,-1,aZ),
			new Vector3(-1 + aBlend, 1-aBlend*2, aZ), new Vector3(1-aBlend, 1-aBlend*2, aZ), new Vector3(1-aBlend, -1+aBlend*2, aZ), new Vector3(-1+aBlend, -1+aBlend*2, aZ) };
		m.colors = new Color[] { new Color(1,1,1,0), new Color(1,1,1,0), new Color(1,1,1,0), new Color(1,1,1,0),
			Color.white, Color.white, Color.white, Color.white };
		m.uv = new Vector2[] { new Vector2(0, 1), new Vector2(1,1), new Vector2(1,0), new Vector2(0,0),
			new Vector2(aBlend/2, 1-aBlend), new Vector2(1-aBlend/2, 1-aBlend), new Vector2(1-aBlend/2, aBlend), new Vector2(aBlend/2, aBlend)};
		m.triangles = new int[] {
			0,1,5,  0,5,4,
			5,1,2,  5,2,6,
			6,2,7,  7,2,3,
			7,3,0,  7,0,4,
			4,5,6,  4,6,7};
		m.RecalculateBounds();

		return m;
	}
	private Mesh CreateNautilusSphere() {
		float overlap = (360 / cSphereColumns);
		float start   = -180 - overlap/2;

		Vector3[] verts  = new Vector3[cSphereRows*cSphereColumns];
		Vector2[] uvs    = new Vector2[verts.Length];
		Color  [] colors = new Color  [verts.Length];
		int    [] tris   = new int    [verts.Length * 6];

		// Generate vertices for the sphere
		for (int y = 0; y < cSphereRows; y++) {
			float yPercent = y / (float)(cSphereRows-1);
			float yAng     = (-Mathf.PI/2f) + yPercent * Mathf.PI;
			for (int x = 0; x < cSphereColumns; x++) {
				float xPercent = x / (float)(cSphereColumns-1);
				float xAng     = (start + xPercent * (360f + overlap*1.1f)) * Mathf.Deg2Rad;
				
				Vector3 pt = new Vector3();
				pt.x = Mathf.Cos(xAng) * Mathf.Cos(yAng);
				pt.z = Mathf.Sin(xAng) * Mathf.Cos(yAng);
				pt.y = Mathf.Sin(yAng);

				int i = x+y*cSphereColumns;
				verts [i] = pt*cSphereSize*(1-xPercent*0.1f); // Expand the sphere as it turns to give it a nautilus shape
				colors[i] = new Color(1,1,1, x==cSphereColumns-1?0:1); // last column gets an alpha of 0, so we get a smooth transition
				uvs   [i] = new Vector2(xPercent, yPercent);
			}
		}

		// now index the verts
		for (int y = 0; y < cSphereRows-1; y++) {
			for (int x = 0; x < cSphereColumns-1; x++) {
				int i = (x + y * cSphereColumns) * 6;
				tris[i]   = x + y * cSphereColumns;
				tris[i+1] = (x+1) + y * cSphereColumns;
				tris[i+2] = (x+1) + (y+1) * cSphereColumns;

				tris[i+3] = x + y * cSphereColumns;
				tris[i+4] = (x+1) + (y+1) * cSphereColumns;
				tris[i+5] = x + (y+1) * cSphereColumns;
			}
		}

		// Build the mesh from the data we've created!
		Mesh m = new Mesh();
		m.vertices  = verts;
		m.colors    = colors;
		m.uv        = uvs;
		m.triangles = tris;
		m.RecalculateBounds();
		return m;
	}
	private bool AngleVisible(Vector3 aDir, Vector3 aCamDir) {
		float angle = Vector3.Angle(aDir, aCamDir);
		return angle < 45 + (_fov/2);
	}
	private void CacheStamp(Vector3 aPosition, Quaternion aOrientation) {
		StampDir stamp = new StampDir();
		stamp.position  = aPosition;
		stamp.direction = aOrientation;
		stamp.timestamp = Time.time;
		_stampCache.Add(stamp);
	}
	private bool IsExpired(int i, Vector3 aCameraPosition) {
		if (_stampExpireLife     > 0 && Time.time - _stampCache[i].timestamp > _stampExpireLife)
			return true;
		if (_stampExpireDistance > 0 && Vector3.SqrMagnitude(_stampCache[i].position - aCameraPosition) > _stampExpireDistance*_stampExpireDistance)
			return true;
		return false;
	}
	#endregion

	#region Static Methods
	public Vector3 GetWeightedDirection(ref Histogram aHistogram) {
		int mipLevel = (int)(Mathf.Log( _map.width ) / Mathf.Log(2)) - 2;
		return GetWeightedDirection(_map, mipLevel, ref aHistogram).normalized;
	}
	public static Vector3 GetWeightedDirection(Cubemap aMap, int aMipLevel, ref Histogram aHistogram) {
		if (aHistogram != null)
			aHistogram.Clear();

		Vector3 result = Vector3.zero;
		result += Quaternion.Euler(0,-90,0) * GetWeightedDirection(aMap, CubemapFace.NegativeX, aMipLevel, ref aHistogram);
		result += Quaternion.Euler(90,0,0)  * GetWeightedDirection(aMap, CubemapFace.NegativeY, aMipLevel, ref aHistogram);
		result += Quaternion.Euler(0,180,0) * GetWeightedDirection(aMap, CubemapFace.NegativeZ, aMipLevel, ref aHistogram);
		result += Quaternion.Euler(0,90,0)  * GetWeightedDirection(aMap, CubemapFace.PositiveX, aMipLevel, ref aHistogram);
		result += Quaternion.Euler(-90,0,0) * GetWeightedDirection(aMap, CubemapFace.PositiveY, aMipLevel, ref aHistogram);
		result += GetWeightedDirection(aMap, CubemapFace.PositiveZ, aMipLevel, ref aHistogram);
		return result/6f;
	}
	static Vector3 GetWeightedDirection(Cubemap aMap, CubemapFace aFace, int aMipLevel, ref Histogram aHistogram) {
		Color[] colors = aMap.GetPixels( aFace, aMipLevel );
		int     width  = Mathf.Max( aMap.width >> aMipLevel );
		float   texelSize = 2f/width;
		float   start = -1f + texelSize/2f;
		float   scale = 1f/(width*width);

		Vector3 result = Vector3.zero;
		for (int y = 0; y < width; y++) {
			float yP = -(start + texelSize * y);
			for (int x = 0; x < width; x++) {
				float xP        = start + texelSize * x;
				Color color = colors[x+y*width];
				float intensity = color.grayscale;
				if (aHistogram != null)
					aHistogram.Add(color.r, color.g, color.b, intensity);
				
				result += new Vector3(xP, yP, 1).normalized * intensity;
			}
		}
		return result * scale;
	}

	public static Texture2D CreateCubemapTex(Cubemap aMap) {
		Texture2D tex  = new Texture2D(aMap.width * 6, aMap.height, TextureFormat.RGB24, false);
		Color[]   data = new Color[tex.width * tex.height];

		CopyFace(ref data, aMap, CubemapFace.PositiveX, 0);
		CopyFace(ref data, aMap, CubemapFace.NegativeX, 1*aMap.width);
		CopyFace(ref data, aMap, CubemapFace.PositiveY, 2*aMap.width);
		CopyFace(ref data, aMap, CubemapFace.NegativeY, 3*aMap.width);
		CopyFace(ref data, aMap, CubemapFace.PositiveZ, 4*aMap.width);
		CopyFace(ref data, aMap, CubemapFace.NegativeZ, 5*aMap.width);

		tex.SetPixels(data);
		tex.Apply();

		return tex;
	}
	static void CopyFace(ref Color[] data, Cubemap aMap, CubemapFace aFace, int startX) {
		Color[] face = aMap.GetPixels(aFace);

		for (int y = 0; y < aMap.height; y++) {
			int yStride = y * aMap.width * 6;
			for (int x = 0; x < aMap.width; x++) {
				int i = startX + x + yStride;
				data[i] = face[x+((aMap.height-1)-y)*aMap.width];
			}
		}
	}
	#endregion
}
