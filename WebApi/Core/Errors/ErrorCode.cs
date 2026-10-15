using Krzaq.Attributes.EnumToString;
using Krzaq.Attributes.HttpStatus;
using System.ComponentModel;
using System.Net;

namespace KrzaqRPG.Backend.WebApi.Core.Errors
{
    [EnumToString(NameAlterMode.ToUpperSnake)]
    public enum ErrorCode
    {
        // +-------------+
        // |   GENERAL   |
        // +-------------+
        [HttpStatus(HttpStatusCode.InternalServerError)]
        [Description("An unknown error occurred")]
        Unknown,

        // +-------------------+
        // |   AUTHORIZATION   |
        // +-------------------+
        [HttpStatus(HttpStatusCode.Unauthorized)]
        [Description("Failed to authorize")]
        Unauthorized,
        [HttpStatus(HttpStatusCode.Forbidden)]
        [Description("Forbidden")]
        Forbidden,
        [HttpStatus(HttpStatusCode.Unauthorized)]
        [Description("Token expired")]
        TokenExpired,
        [HttpStatus(HttpStatusCode.Unauthorized)]
        [Description("Invalid token")]
        InvalidToken,
        [HttpStatus(HttpStatusCode.Conflict)]
        [Description("Token already exists")]
        TokenExists,

        // +----------------+
        // |   GAME LOGIC   |
        // +----------------+

        // --- Character ---
        [HttpStatus(HttpStatusCode.Conflict)]
        [Description("Name is already in use")]
        NameAlreadyInUse,
        [HttpStatus(HttpStatusCode.NotFound)]
        [Description("Character not found")]
        CharacterNotFound,
        [HttpStatus(HttpStatusCode.Conflict)]
        [Description("Character is currently traveling")]
        CharacterInTravel,
        [HttpStatus(HttpStatusCode.Conflict)]
        [Description("Character is outside any area")]
        CharacterOutsideArea,

        // --- Area ---
        [HttpStatus(HttpStatusCode.NotFound)]
        [Description("Area not found")]
        AreaNotFound,

        // --- Path ---
        [HttpStatus(HttpStatusCode.NotFound)]
        [Description("Path not found")]
        PathNotFound,

        // --- Travel ---
        [HttpStatus(HttpStatusCode.Conflict)]
        [Description("Invalid travel paths has been found: [{0}]")]
        InvalidTravelPaths
    }
}
