﻿using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Bloggy.TagHelpers
{
    [HtmlTargetElement("gravatar", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class GravatarTagHelper : TagHelper
    {
        private int _size = 80;
        private GravatarRating _rating = GravatarRating.PG;

        private static readonly int MinimumSize = 1;
        private static readonly int MaximumSize = 512;

        private const string GravatarUrl = "http://www.gravatar.com/avatar/";

        public string DefaultImage { get; set; }

        public string Email { get; set; }

        public GravatarRating Rating
        {
            get => GravatarRating.PG;
            set => _rating = value;
        }

        public int Size
        {
            get => _size;
            set
            {
                if (value < MinimumSize || value > MaximumSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(Size), $"The size should be between {MinimumSize} - {MaximumSize}.");
                }

                _size = value;
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null;

            var hash = ComputeHash(Email);
            var imageUrl = $"{GravatarUrl}{hash}?s={Size}&r={Rating}";

            if (!string.IsNullOrEmpty(DefaultImage))
            {
                imageUrl += $"&d={DefaultImage}";
            }

            output.Content.SetHtmlContent($"<img src=\"{imageUrl}\" />");
        }

        private string ComputeHash(string email)
        {
            var md5 = MD5.Create();
            var bytes = Encoding.ASCII.GetBytes(email);
            var hash = md5.ComputeHash(bytes);
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}