﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public class DataSourceReflectionListTest : MonoBehaviour, IDataSourceProvider
    {

        [Tooltip("The number of random image entries to generate for image collection.")]
        [SerializeField]
        protected int _staticCollectionSize = 200;

        [Tooltip("The maximum number of entries for the fuctuating image collection.")]
        [SerializeField]
        protected int _fluxCollectionSize = 20;


        protected int _fluxImageIndex = 0;
        protected bool _fluxImageAdding = true;
        private float _deltaSeconds;
        private int _nextUpdateTarget;

        /// <summary>
        /// A class to contain data that is to be used as a data source. This is akin to a view model
        /// that will be used to populate a view.
        /// </summary>
        private class ImageInfo
        {
            public string id;
            public string title;
            public string description;
            public string imageUrl;
        }

        private class TestInfo
        {
            public ObservableCollection<ImageInfo> staticImages = new ObservableCollection<ImageInfo>();
            public ObservableCollection<ImageInfo> fluxImages = new ObservableCollection<ImageInfo>();

        }


        private DataSourceReflection _dataSource;
        private TestInfo _dataSourceObject = new TestInfo();

        /// <summary>
        /// IDataSourceProvider method used to provide the correct
        /// datasource, which in this case is the DataSourceReflection instance with
        /// the specified _dataSourceObject attached to it to provide the actual
        /// data using reflection.
        /// </summary>


        public IDataSource GetDataSource()
        {

            if (_dataSource == null)
            {
                _dataSource = new DataSourceReflection(_dataSourceObject);
            }

            return _dataSource;
        }

        private void OnEnable()
        {
            GetDataSource();
            InitializeData();
        }


        private void Update()
        {
            if (_fluxCollectionSize > 0)
            {
                _deltaSeconds += Time.deltaTime;

                int tenthsOfSeconds = (int)(_deltaSeconds * 10.0);

                if (tenthsOfSeconds > _nextUpdateTarget)
                {
                    _nextUpdateTarget += 5;
                    if (_fluxImageAdding)
                    {
                        _dataSourceObject.fluxImages.Add(_dataSourceObject.staticImages[_fluxImageIndex++]);
                        if (_fluxImageIndex >= _fluxCollectionSize)
                        {
                            _fluxImageIndex = _fluxCollectionSize;
                            _fluxImageAdding = false;
                        }
                    }
                    else
                    {
                        _dataSourceObject.fluxImages.RemoveAt(--_fluxImageIndex);
                        if (_fluxImageIndex <= 0)
                        {
                            _fluxImageIndex = 0;
                            _fluxImageAdding = true;
                        }
                    }
                }
            }

        }


        private void InitializeData()
        {
            _dataSource.DataChangeSetBegin();

            InitializeImageList();
           // _dataSource.NotifyAllChanged();
            _dataSource.DataChangeSetEnd();
        }


        protected void InitializeImageList()
        {
            int maxImageId = 101;  // highest image id of available photos. used in url construction

            string[] dates = { "February 26, 2020", "January 1, 2000", "March 1, 2018", "April 29, 2017", "May 5, 1999", "June 21, 1980" };
            string[] words = { "mixed reality", "MRTK", "HoloLens", "Unity", "Visual Studio", "Middleware", "Microsoft", "XR", "VR", "AR", "MR", "C#", "Azure" };

            if (_dataSourceObject.staticImages == null)
            {
                _dataSourceObject.staticImages = new ObservableCollection<ImageInfo>();
            }

            System.Random r = new System.Random();

            for (int i = 0; i < _staticCollectionSize; i++)
            {
                ImageInfo imageInfo = new ImageInfo();

                int imageId = 1 + (i % maxImageId);
                string pathBase = String.Format("images[{0:d}]", i);
                imageInfo.id = String.Format("{0:D6}", i);
                imageInfo.title = String.Format("Image #{0:d}", i);
                imageInfo.imageUrl = String.Format("http://michaelinfo.com/nature-photos/images/{0:D4}.jpg", imageId);

                Randomize(words);
                imageInfo.description = "";
                for (int w = 0; w < 5; w++)
                {
                    imageInfo.description = imageInfo.description + words[w] + " ";
                }

                _dataSourceObject.staticImages.Add(imageInfo);
            }
        }

        protected void Randomize(string[] items)
        {
            System.Random rand = new System.Random();

            // For each spot in the array, pick
            // a random item to swap into that spot.
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = rand.Next(i, items.Length);
                string temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }
    }
}
