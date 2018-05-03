using System;
using System.Collections.Generic;


namespace QiubaiCrawler.Models
{
    public class QiuBaiModel
    {
        public QiuBaiModel()
        {
            
        }
        public string UserId { get; set; }
        public string UserIcon { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string Content { get; set; }
        public bool HasImage { get; set; }
        public string ImageUrl { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public override string ToString()
        {
            return $"userId: {UserId} \n userName: {UserName} \n UserIcon: {UserIcon} \n Gender: {Gender} \n Age: {Age} \n content: \n {Content} \n ImageUrl: {ImageUrl}";
        }
    }

    public class Comment
    {
        public Comment() { }

        public string UserId { get; set; }
        public string UserIcon { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string Content { get; set; }
    }
}