using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Influunt.Host.ViewModels
{
    /// <summary>
    /// News channel
    /// </summary>
    public class FeedChannelViewModel : IValidatableObject
    {
        /// <summary>
        /// Url for getting news
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Channel name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Show in infinity feed flag
        /// </summary>
        public bool Hidden { get; set; } = false;

        /// <summary>
        /// Unique Identifier
        /// </summary>
        public string Id { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            if (string.IsNullOrWhiteSpace(Name)
                || string.IsNullOrWhiteSpace(Url))
                validationResults.Add(new ValidationResult("Can't be null or empty",
                    new[] {nameof(Name), nameof(Url)}));

            if (!Url.StartsWith("http://")
                && !Url.StartsWith("https://"))
                validationResults.Add(new ValidationResult("Should start with http or https", new[] {nameof(Url)}));

            if(!validationResults.Any())
                validationResults.Add(ValidationResult.Success);

            return validationResults;
        }
    }
}