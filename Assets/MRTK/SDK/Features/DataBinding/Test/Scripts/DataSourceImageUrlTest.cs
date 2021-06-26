// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;




namespace Microsoft.MixedReality.Toolkit.Data
{
    /// <summary>
    /// Test data source which programmatically triggers an update of a url that changes
    /// its reported content, in this case an image that populates a sprite. 
    /// </summary>
    /// 
    /// <remarks>
    /// Using a simple <key,value> store, it's possible to separate data from view
    /// to simplify the integration of generic view prefabs that are populated from
    /// external information.
    /// </remarks>
    
    public class DataSourceImageUrlTest : DataSourceGOBase
    {

        [Tooltip("A URL for a cloud service that provides a random URL in return, typically for an image.")]
        [SerializeField]
        private string url = "https://picsum.photos/800/600";

        [Tooltip("Time to wait between fetches of the next random URL.")]
        [SerializeField]
        private float secondsBetweenFetches = 5.0f;

        [Tooltip("The keypath of a datum in the data set that will receive the URL provided by the cloud service as its response.")]
        [SerializeField]
        private string keyPath = "imageUrl";

        protected float m_time = 0.0f;
        protected int m_versionCounter = 0;


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
            m_time = secondsBetweenFetches;
        }


        // Update is called once per frame
        protected void Update()
        {
            m_time += Time.deltaTime;

            if (m_time >= secondsBetweenFetches)
            {
                m_time -= secondsBetweenFetches;

                // ensure we don't cause a local cache hit by changing an ignored fake version
                SetValue(keyPath, url + "?v=" + (++m_versionCounter).ToString());
            }
        }
    }
}

