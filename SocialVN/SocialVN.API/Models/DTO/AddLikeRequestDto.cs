﻿namespace SocialVN.API.Models.DTO
{
    public class AddLikeRequestDto :BaseDto
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
