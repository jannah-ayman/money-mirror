using MoneyMirror.Core.Enums.CharacterEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.DTOs.Character
{
    /// <summary>
    /// Data Transfer Object for requesting character state.
    /// Used as input for POST /api/character/state endpoint.
    /// </summary>
    public class GetCharacterStateDto
    {
        /// <summary>
        /// Current screen/context the child is viewing.
        /// Used to determine appropriate character state.
        /// </summary>
        public ScreenContext ScreenContext { get; set; }

        /// <summary>
        /// Optional: Additional context data for state determination.
        /// Example: current balance, goal progress percentage, etc.
        /// Stored as JSON for flexibility.
        /// </summary>
        public string? ContextData { get; set; }
    }
}
