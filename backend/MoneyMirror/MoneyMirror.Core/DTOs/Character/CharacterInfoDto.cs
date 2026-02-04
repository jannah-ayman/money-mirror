using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.DTOs.Character
{
    /// <summary>
    /// Data Transfer Object for available character information.
    /// Returns character details for selection screen.
    /// </summary>
    public class CharacterInfoDto
    {
        /// <summary>
        /// Character type identifier.
        /// Example: "Nova", "Luna", "Cosmo", "Aura"
        /// </summary>
        public string CharacterType { get; set; }

        /// <summary>
        /// Display name of the character.
        /// Example: "Nova the Explorer"
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Short description of character personality.
        /// Example: "Energetic and loves adventures!"
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL to idle state image (preview for selection).
        /// Example: "/characters/nova/idle.png"
        /// </summary>
        public string PreviewImageUrl { get; set; }
    }
}
