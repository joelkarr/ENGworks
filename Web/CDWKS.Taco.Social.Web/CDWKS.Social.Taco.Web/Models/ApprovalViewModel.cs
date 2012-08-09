using System.Collections.Generic;

namespace CDWKS.Social.Taco.Models
{
    public class ApprovalViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool LoginFail { get; set; }
        public List<SocialFeedbackForm> SocialFeedbackFormList { get; set; }
    }
}