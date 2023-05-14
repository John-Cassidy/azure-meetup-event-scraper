using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azure_meetup_event_scraper {
    public class GroupEvent {
        public int Id { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }

        public EventType Type { get; set; }
    }

    public enum EventType {
        NBA = 1,
        VBA = 2,
        BA = 3
    }
}
