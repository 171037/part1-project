using System;
using System.Collections.Generic;

namespace recipe_in_home.Models
{
    public class Post
    {
        public int Postid { get; set; }
        public int member_id { get; set; }
        public string member_name { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageData { get; set; }
        public string ImagePath { get; set; } // 이미지 경로를 저장
        public string CreatedAt { get; set; }
        //public List<comments> Comments { get; set; }
    }
}
