using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Eksamen2024.Controllers;
using Eksamen2024.DAL;
using Eksamen2024.Models;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;


namespace Eksamen2024.Controllers;


    public class UsersControllerTests
    {
    [Fact]
    // Testing that create user method works.
    public void CreateUsers()
    {
        // Given
        var user = new User
        {
            UserId = 1,
            Username = "Moren Din",
            Email = "Farendin@morendin.com",
            HashedPassword = "Farendin123!"
        };
        // Then
        Assert.Equal(1, user.UserId);
        Assert.Equal("Moren Din", user.Username);
        Assert.Equal("Farendin@morendin.com", user.Email);
        Assert.Equal("Farendin123!", user.HashedPassword);

    }

    [Fact]
    public void CreateUsers_NotworkingUsername()
    {
        // Given
         var user = new User
        {
            UserId = 1,
            Email = "Farendin@morendin.com",
            HashedPassword = "Farendin123!"
        };

        var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(user);
            bool isValid = Validator.TryValidateObject(user, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            // Null-check because of warning
            if (validationResults != null && validationResults.Any())
                {
                    Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Username"));
                }

        }
    [Fact]
    public void CreateUsers_NotworkingEmail()
    {
        // Given
         var user = new User
        {
            UserId = 1,
            Username = "HAHAHAHA",
            HashedPassword = "Farendin123!"
        };

        var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(user);
            bool isValid = Validator.TryValidateObject(user, context, validationResults, true);

            // Assert
            Assert.False(isValid);
            // Null-check because of warning
            if (validationResults != null && validationResults.Any())
                {
                    Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Email"));
                }

        }
        [Fact]
    public void CreateUsers_NotworkingPassword()
    {
        // Given
         var user = new User
        {
            UserId = 1,
            Email = "Farendin@morendin.com",
            Username = "HAHAHAHA",
        };

            var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(user);
    bool isValid = Validator.TryValidateObject(user, context, validationResults, true);

    // Assert
    Assert.False(isValid);
    // Null-check because of warning
    if (validationResults != null && validationResults.Any())
    {
        Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Password is required"));
    }
    else
    {
        Assert.Fail("ValidationResults was null or empty");
    }
        }
    }
