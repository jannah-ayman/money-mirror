using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.DTOs.Character
{
    /// <summary>
    /// Response after selecting a character.
    /// </summary>
    public class SelectCharacterResponseDto
    {
        /// <summary>
        /// Selected character type.
        /// </summary>
        public string CharacterType { get; set; }

        /// <summary>
        /// URL to character's idle state (profile picture).
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Success message.
        /// </summary>
        public string Message { get; set; }
    }
}
