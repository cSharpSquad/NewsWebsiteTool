﻿namespace NewsWebApplication.DTO
{
    public class AuthorDto
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }

    public class AuthorNewsCountDto
    {
        public long AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int NewsCount { get; set; }
    }
}
