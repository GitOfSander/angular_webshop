using Site.Data;
using Site.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using static Site.Startup;
using Microsoft.Extensions.Options;
using Site.Models.App;

namespace Site.Models
{
    public class Review : Controller
    {
        SiteContext _context;
        private readonly IOptions<AppSettings> _config;

        public Review(SiteContext context, IOptions<AppSettings> config)
        {
            _context = context;
            _config = config;
        }

        public class ReviewBundle
        {
            public Reviews Review { get; set; }
            public IEnumerable<ReviewResources> ReviewResources { get; set; }
            public IEnumerable<ReviewTemplateFields> ReviewTemplateFields { get; set; }
            public ReviewTemplates ReviewTemplate { get; set; }
        }

        public bool InsertReview(string CallName, int WebsiteLanguageId, int LinkedToId, string UserId, string Name, string Email, string Text, byte Rating)
        {
            ReviewTemplates _reviewTemplate = _context.ReviewTemplates.Where(x => x.WebsiteId == 1).FirstOrDefault(x => x.CallName == CallName);

            DateTime UtcTime = DateTime.UtcNow;
            TimeZoneInfo Tzi = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
            DateTime CreatedAt = TimeZoneInfo.ConvertTime(UtcTime, Tzi); // convert from utc to local

            bool Active = true;
            if (_reviewTemplate.CheckBeforeOnline == true)
            {
                Active = false;
            }

            var _review = new Reviews { WebsiteLanguageId = WebsiteLanguageId, LinkedToId = LinkedToId, UserId = UserId, Name = Name, Email = Email, Text = Text, Rating = Rating, Active = Active, CreatedAt = CreatedAt, ViewedByAdmin = false, ReviewTemplateId = _reviewTemplate.Id };
            _context.Reviews.Add(_review);
            _context.SaveChanges();

            return true;
        }

        public List<ReviewBundle> GetReviewBundles(int websiteId, string callName)
        {
            return _context.Reviews.Join(_context.ReviewTemplates, Reviews => Reviews.ReviewTemplateId, ReviewTemplates => ReviewTemplates.Id, (Reviews, ReviewTemplates) => new { Reviews, ReviewTemplates })
                                   .GroupJoin(_context.ReviewResources.Join(_context.ReviewTemplateFields, ReviewResources => ReviewResources.ReviewTemplateFieldId, ReviewTemplateFields => ReviewTemplateFields.Id, (ReviewResources, ReviewTemplateFields) => new { ReviewResources, ReviewTemplateFields }), x => x.Reviews.Id, x => x.ReviewResources.ReviewId, (x, ReviewResources) => new { x.Reviews, x.ReviewTemplates, ReviewResources })
                                   .Where(x => x.Reviews.Active == true)
                                   .Where(x => x.ReviewTemplates.WebsiteId == websiteId)
                                   .Where(x => x.ReviewTemplates.CallName == callName)
                                   .Select(x => new ReviewBundle() {
                                       Review = x.Reviews,
                                       ReviewResources = x.ReviewResources.Select(y => y.ReviewResources),
                                       ReviewTemplateFields = x.ReviewResources.Select(y => y.ReviewTemplateFields),
                                       ReviewTemplate = x.ReviewTemplates
                                   }).ToList();
        }

        public ReviewBundle ChangeReviewBundleTextByType(ReviewBundle reviewBundle)
        {
            reviewBundle.Review.Text = reviewBundle.Review.Text.Replace("\r\n", "\n").Replace("\n", "<br />");

            if (reviewBundle.ReviewResources != null)
            {
                foreach (ReviewResources reviewResource in reviewBundle.ReviewResources)
                {
                    string type = reviewBundle.ReviewTemplateFields.FirstOrDefault(ReviewTemplateFields => ReviewTemplateFields.Id == reviewResource.ReviewTemplateFieldId).Type;
                    string text = reviewResource.Text;
                    if (type.ToLower() == "textarea")
                    {
                        reviewBundle.ReviewResources.FirstOrDefault(ReviewResources => ReviewResources.Id == reviewResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                    }
                }
            }

            return reviewBundle;
        }

        public List<ReviewBundle> ChangeReviewBundlesTextByType(List<ReviewBundle> reviewBundles)
        {
            foreach (ReviewBundle reviewBundle in reviewBundles)
            {
                reviewBundles.FirstOrDefault(x => x.Review.Id == reviewBundle.Review.Id).Review.Text = reviewBundle.Review.Text.Replace("\r\n", "\n").Replace("\n", "<br />");

                if (reviewBundle.ReviewResources != null)
                {
                    foreach (ReviewResources reviewResource in reviewBundle.ReviewResources)
                    {
                        string type = reviewBundle.ReviewTemplateFields.FirstOrDefault(ReviewTemplateFields => ReviewTemplateFields.Id == reviewResource.ReviewTemplateFieldId).Type;
                        string text = reviewResource.Text;
                        if (type.ToLower() == "textarea")
                        {
                            reviewBundles.FirstOrDefault(x => x.Review.Id == reviewBundle.Review.Id).ReviewResources.FirstOrDefault(ReviewResources => ReviewResources.Id == reviewResource.Id).Text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                        }
                    }
                }
            }

            return reviewBundles;
        }

        public List<Dictionary<string, object>> ConvertReviewBundlesToJson(List<ReviewBundle> reviewBundles)
        {
            string url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId);

            List<Dictionary<string, object>> reviewBundlesList = new List<Dictionary<string, object>>();
            foreach (ReviewBundle reviewBundle in reviewBundles)
            {
                reviewBundlesList.Add(ConvertReviewBundleToJson(reviewBundle, url));
            }

            return reviewBundlesList;
        }

        public Dictionary<string, object> ConvertReviewBundleToJson(ReviewBundle reviewBundle, string url = "")
        {
            if (url == "") { url = new Website(_context, _config).GetWebsiteUrl(_config.Value.WebsiteId); }

            Dictionary<string, object> fields = new Dictionary<string, object>();
            foreach (ReviewTemplateFields reviewTemplateField in reviewBundle.ReviewTemplateFields.Distinct())
            {
                if (reviewTemplateField.Type.ToLower() != "selectlinkedto")
                {
                    string text = reviewBundle.ReviewResources.FirstOrDefault(ReviewResources => ReviewResources.ReviewTemplateFieldId == reviewTemplateField.Id).Text;
                    if (reviewTemplateField.Type.ToLower() != "textarea")
                    {
                        text = text.Replace("\r\n", "\n").Replace("\n", "<br />");
                    }

                    fields.Add(reviewTemplateField.CallName, text);
                }
            }

            return new Dictionary<string, object>() {
                { "name", reviewBundle.Review.Name },
                { "email", reviewBundle.Review.Email },
                { "text", reviewBundle.Review.Text },
                { "rating", reviewBundle.Review.Rating },
                { "createdAt", reviewBundle.Review.CreatedAt },
                { "anonymous", reviewBundle.Review.Anonymous },
                { "active", reviewBundle.Review.Active },
                { "resources", fields }
            };
        }

        [Route("/spine-api/review-bundles")]
        [HttpGet]
        public IActionResult GetReviewBundlesApi(int websiteId, string callName)
        {
            try
            {
                return Ok(ConvertReviewBundlesToJson(GetReviewBundles(websiteId, callName)));
            }
            catch
            {
                return StatusCode(400);
            }
        }
    }
}
