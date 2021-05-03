using System;
using System.Collections.Generic;

namespace SampleJsonGenerator
{
    public class CourseView
    {
        public int userId { get; set; }

        public string user { get; set; }

        public string course { get; set; }

        public DateTime watchedDate { get; set; }

        public int secondsWatched { get; set; }
    }

    public class CourseViewList
    {
        public List<CourseView> courseViews { get; set; }

        public CourseViewList()
        {
            courseViews = new List<CourseView>();
        }


    }
}