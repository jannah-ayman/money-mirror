using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.DTOs.Character
{
    /// <summary>
    /// Data Transfer Object for character state response.
    /// Returns character image URL and state information.
    /// </summary>
    public class CharacterStateResponseDto
    {
        /// <summary>
        /// Current character type.
        /// Example: "Nova"
        /// </summary>
        public string CharacterType { get; set; }

        /// <summary>
        /// Current state being displayed.
        /// Example: "Happy", "Encouraging", "Thinking"
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// URL to the character image for this state.
        /// Example: "/characters/nova/happy.png"
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Optional dialogue/message from character.
        /// Example: "Great job saving this week! 🌟"
        /// </summary>
        public string? Message { get; set; }
    }
}
