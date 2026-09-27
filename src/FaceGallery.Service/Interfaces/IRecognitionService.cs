using c__service_2026.Dto;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace c__service_2026.Interfaces
{
    /// <summary>
    /// The core feature: given a reference photo of one specific person,
    /// returns every image (in one gallery, or across everything one user
    /// owns) ranked by how similar it is to the reference photo - i.e.
    /// "every photo this person appears in".
    /// </summary>
    public interface IRecognitionService
    {
        /// <summary>
        /// Search a single gallery. matchThreshold: null = use the active
        /// IFaceMatchingService's own SuggestedMatchThreshold.
        /// </summary>
        Task<List<MatchResultDto>> FindMatchesAsync(Stream referencePhoto, int galleryId, double? matchThreshold = null);

        /// <summary>
        /// Search across every gallery/image that belongs to one user, instead
        /// of picking a gallery first.
        /// </summary>
        Task<List<MatchResultDto>> FindMatchesForUserAsync(Stream referencePhoto, int userId, double? matchThreshold = null);
    }
}
