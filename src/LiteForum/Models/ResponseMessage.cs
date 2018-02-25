
namespace LiteForum.Models {
    public class LiteForumResponseMessage {
        public int StatusCode { get; }
        public string Message { get; }

        public LiteForumResponseMessage(int statusCode, string message) {
            this.StatusCode =statusCode;
            this.Message = message;
        }

        public LiteForumResponseMessage(string message) {
            this.Message = message;
        }
    }
}
