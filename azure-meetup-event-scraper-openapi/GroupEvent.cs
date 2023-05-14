using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azure_meetup_event_scraper_openapi {
    public class GroupEvent {
        public int Id { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }

        public EventType EventTypeId { get; set; }
        public string EventTypeName { get; set; }
    }

    public enum EventType {
        NBA = 1,
        VBA = 2,
        BA = 3
    }
}
