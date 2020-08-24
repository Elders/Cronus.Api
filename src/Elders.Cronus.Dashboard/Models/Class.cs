//using System;
//using System.Collections.Generic;

//namespace Elders.Cronus.Dashboard.Models
//{
//    public class AggregateDto
//    {
//        public AggregateDto()
//        {
//            Commits = new List<AggregateCommitDto>();
//        }

//        public string BoundedContext { get; set; }

//        public string AggregateId { get; set; }

//        public List<AggregateCommitDto> Commits { get; set; }
//    }

//    public class AggregateCommitDto
//    {
//        public AggregateCommitDto()
//        {
//            Events = new List<EventDto>();
//        }

//        public int AggregateRootRevision { get; set; }

//        public List<EventDto> Events { get; set; }

//        public DateTime Timestamp { get; set; }
//    }

//    public class EventDto
//    {
//        public string EventName { get; set; }

//        public object EventData { get; set; }
//    }
//}
