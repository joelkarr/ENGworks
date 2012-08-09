﻿using System.ComponentModel.DataAnnotations;

namespace CDWKS.Social.Taco.Models
{
    /// <summary>
    /// Extended SocialFeedbackForm class just used to add the MetadataType class-level attribute, because the types generated by EF should be partial, 
    /// thus you can add a bit more to them like the MetadataTypeAttribute and other custom properties.
    /// </summary>
    [MetadataType(typeof(SocialFeedbackFormMetaData))]
    public partial class SocialFeedbackForm
    {
        public bool Dislike { get; set; }
    }

    /// <summary>
    /// User meta data for the User partial class generated by EF.
    /// </summary>
    public class SocialFeedbackFormMetaData
    {
        [MaxLength(50)]
        [Display(Name = "Username:")]
        public string Username { get; set; }

        [MaxLength(150)]
        [Display(Name = "Email Address:")]
        public string Email { get; set; }

        [MaxLength(50)]
        [Display(Name = "First Name (Optional):")]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [Display(Name = "Last Name (Optional):")]
        public string LastName { get; set; }

        [MaxLength(100)]
        [Display(Name = "Company (Optional):")]
        public string Company { get; set; }

        [MaxLength(100)]
        [Display(Name = "Product:")]
        public string Product { get; set; }

        [MaxLength(100)]
        [Display(Name = "Family:")]
        public string Family { get; set; }

        [Display(Name = "Like")]
        public bool Like { get; set; }
        
        [Display(Name = "Don't Like")]
        public bool Dislike { get; set; }

        [MaxLength(120)]
        [Display(Name = "Comments:")]
        public string Comments { get; set; }
    }
}