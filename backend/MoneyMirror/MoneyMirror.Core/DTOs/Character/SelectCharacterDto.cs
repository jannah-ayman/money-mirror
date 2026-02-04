using MoneyMirror.Core.Enums.CharacterEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMirror.Core.DTOs.Character
{
    /// <summary>
    /// Data Transfer Object for updating child's selected character.
    /// Used as input for PUT /api/character/select endpoint.
    /// </summary>
    public class SelectCharacterDto
    {
        /// <summary>
        /// Character type to select.
        /// Must be one of: Nova, Luna, Cosmo, Aura
        /// </summary>
        public CharacterType CharacterType { get; set; }
    }
}
