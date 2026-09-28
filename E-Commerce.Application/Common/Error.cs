using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public sealed record Error (string code,string description,ErrorType Type = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string description = " A General Failure Has Occurred")
    => new Error(code, description, ErrorType.Failure);

        public static Error Validation(string code = "General.Validation", string description = " A Validation Error Has Occurred")
            => new Error(code, description, ErrorType.Validation);

        public static Error NotFound(string code = "General.NotFound", string description = " The Request Resource was not found")
            => new Error(code, description, ErrorType.NotFound);

        public static Error Conflict(string code = "General.Conflict", string description = " A Conflict Occured in the current state")
            => new Error(code, description, ErrorType.Conflict);

        public static Error Unauthorized(string code = "General.Unauthorized", string description = " Access denied due to lack of Authorization")
            => new Error(code, description, ErrorType.Unauthorized);

        public static Error Forbidden(string code = "General.Forbidden", string description = " This operation is Forbidden")
            => new Error(code, description, ErrorType.Forbidden);

        public static Error InvalidCredentails(string code = "General.InvalidCredentails", string description = " The provided credentials are not valid")
            => new Error(code, description, ErrorType.InvalidCredtials);
    }
}
