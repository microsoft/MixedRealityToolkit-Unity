// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Data
{




    /// <summary>
    /// Simple test data source that programmatically changes variables in a data source. 
    /// </summary>
    /// 
    /// <remarks>
    /// Using a simple <key,value> store, it's possible to separate data from view
    /// to simplify the integration of generic view prefabs that are populated from
    /// external information.
    /// </remarks>

    public class DataSourceDictionaryTest : DataSourceGOBase
    {
        private float m_deltaSeconds;
        private int m_nextOneSecondTarget;
        private int m_nextFiveSecondTarget;
        private bool m_styleYellow;
        private int m_status = 0;
        private int m_score = 100;

        public override IDataSource GetDataSource()
        {
            if (m_dataSource == null)
            {
                m_dataSource = new DataSourceDictionary();
            }

            return m_dataSource;
        }

        protected override void InitializeDataSource()
        {
            InitializeData();
            m_deltaSeconds = 0;
            m_nextOneSecondTarget = 0;
        }

        // Update is called once per frame
        protected void Update()
        {
            string[] statusText = { "open", "pending", "cancelled", "inprogress", "completed" };
            m_deltaSeconds += Time.deltaTime;
            int tenthsOfSeconds = (int)(m_deltaSeconds * 10.0);

            DataChangeSetBegin();


            if (tenthsOfSeconds > m_nextFiveSecondTarget )
            {
                if ( m_styleYellow)
                {
                    SetValue("stylesheet", "standard");
                } else
                {
                    SetValue("stylesheet", "yellow");
                }
                m_styleYellow = !m_styleYellow;
                m_nextFiveSecondTarget += 50;
            }

            if ( tenthsOfSeconds > m_nextOneSecondTarget)
            {
                m_nextOneSecondTarget += 10;
                SetValue("score", ++m_score);

                if ((++m_status % statusText.Length) == 0)
                {
                    m_status = 0;
                }

                SetValue("status", statusText[m_status]);
            }

            DataChangeSetEnd();

        }

        private void InitializeData()
        {
            DataChangeSetBegin();
            SetValue("firstname", "Michael");
            SetValue("lastname", "Hoffman");
            SetValue("score", 123);
            SetValue("status", "open");
            SetValue("stylesheet", "standard");
            DataChangeSetEnd();
        }
    }
}
