// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Blend
{
    public class BlendText : Blend<string>
    {
        private TextMesh textMesh;
        private Text text;

        private Vector2 range = new Vector2(32, 125);
        private Vector2 symbol1 = new Vector2(32, 47);
        private Vector2 numbers = new Vector2(48, 57);
        private Vector2 symbol2 = new Vector2(58, 64);
        private Vector2 upper = new Vector2(65, 90);
        private Vector2 symbol3 = new Vector2(91, 96);
        private Vector2 lower = new Vector2(97, 122);
        private Vector2 symbol4 = new Vector2(123, 125);

        private Vector2[] ranges;

        protected override void Awake()
        {
            ranges = new Vector2[] { symbol1, numbers, symbol2, upper, symbol3, lower, symbol4 };

            textMesh = GetComponent<TextMesh>();
            text = GetComponent<Text>();
            
            base.Awake();
        }

        public override bool CompareValues(string value1, string value2)
        {
            return value1 == value2;
        }

        /// <summary>
        /// get the currrent string in the text object
        /// </summary>
        /// <returns></returns>
        public override string GetValue()
        {
            if (textMesh != null)
            {
                return textMesh.text;
            }
            else if(text != null)
            {
                return text.text;
            }

            return "";
        }

        /// <summary>
        /// tween the strings
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public override string LerpValues(string startValue, string targetValue, float percent)
        {
            // handle letter count
            int count = Mathf.RoundToInt(startValue.Length + (targetValue.Length - startValue.Length) * percent);
            
            string newString = "";
            char[] startArray = startValue.ToCharArray();
            char[] targetArray = targetValue.ToCharArray();
            // handle letters
            for (int i = 0; i < count; i++)
            {
                char start = ' ';
                char target = ' ';
                if (i < startArray.Length)
                {
                    start = startArray[i];
                }
                else
                {
                    start = GetRandomChar(targetArray[i]);
                }

                if (i < targetArray.Length)
                {
                    target = targetArray[i];
                }
                else
                {
                    target = GetRandomChar(startArray[i]);
                    
                }

                newString += CharValue(start, target, percent);
            }

           // print(newString);
            
            return newString;
        }

        /// <summary>
        /// handle the char value transitions
        /// </summary>
        /// <param name="start"></param>
        /// <param name="target"></param>
        /// <param name="perc"></param>
        /// <returns></returns>
        public string CharValue(char start, char target, float perc)
        {
            if (perc == 0) return start.ToString();
            if (perc == 1) return target.ToString();

            if (start == 32)
            {
                start = GetRandomChar(target);
            }

            if(target == 32)
            {
                target = GetRandomChar(start);
            }

            start = (char)Mathf.Clamp(start, range.x, range.y);
            target = (char)Mathf.Clamp(target, range.x, range.y);

            char newChar = (char)GetIndex(start, target, perc);
            return newChar.ToString();
        }

        /// <summary>
        /// figure out what char spans the transition will cross
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="perc"></param>
        /// <returns></returns>
        private int GetIndex(int start, int end, float perc)
        {
            int startIndex = -1;
            int endIndex = -1;
            float span = end - start;
            int range = 0;

            Vector2 startRange = ranges[0];
            Vector2 endRange = ranges[0];
            int startSpan = 0;
            int endSpan = 0;

            int outInt = 0;

            for (int i = 0; i < ranges.Length; i++)
            {
                if (InRange(start, (int)ranges[i].x, (int)ranges[i].y))
                {
                    startIndex = i;
                    startRange = ranges[i];
                }

                if (InRange(end, (int)ranges[i].x, (int)ranges[i].y))
                {
                    endIndex = i;
                    endRange = ranges[i];
                }
            }

            if (startIndex == endIndex)
            {
                outInt = (int)(start + span * perc);
                return outInt;
            }
            else
            {
                if (span < 0)
                {
                    startSpan = (start - (int)startRange.x);
                    endSpan = ((int)endRange.y - end);

                    range = -(startSpan + endSpan);
                    outInt = (int)(start + range * perc);

                    if (outInt < start - startSpan)
                    {
                        outInt = (int)(endRange.y - (startSpan - range * perc));
                    }

                    return outInt;
                }
                else
                {
                    startSpan = ((int)startRange.y - start);
                    endSpan = (end - (int)endRange.x);
                    range = startSpan + endSpan;

                    outInt = (int)(start + range * perc);

                    if (outInt > start + startSpan)
                    {
                        outInt = (int)(endRange.x + (startSpan - range * perc));
                    }

                    return outInt;
                }
            }
        }

        /// <summary>
        /// if no values are available, like when the target string is smaller thant the start string
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private char GetRandomChar(int index)
        {
            int randomIndex = 0;

            for (int i = 0; i < ranges.Length; i++)
            {
                if (InRange(index, (int)ranges[i].x, (int)ranges[i].y))
                {
                    randomIndex = (int)(Random.value * (ranges[i].y - ranges[i].x) + ranges[i].x);
                }
            }

            return (char)randomIndex;
        }

        /// <summary>
        /// is the current int within the min and max
        /// </summary>
        /// <param name="point"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private bool InRange(int point, int min, int max)
        {
            return point >= min && point <= max;
        }

        /// <summary>
        /// is the start and end values within the same range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private bool InRange2(int start, int end, int min, int max)
        {
            return start >= min && start <= max && end >= min && end <= max;
        }

        /// <summary>
        /// ether the start or end is within this range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private bool HasRange(int start, int end, int min, int max)
        {
            return (start >= min && start <= max) || (end >= min && end <= max);
        }
        
        /// <summary>
        /// apply the new string to the text object
        /// </summary>
        /// <param name="value"></param>
        public override void SetValue(string value)
        {
            if (textMesh != null)
            {
                textMesh.text = value;
            }
            else if (text != null)
            {
                text.text = value;
            }
        }
    }
}
