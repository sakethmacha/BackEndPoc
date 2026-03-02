
namespace MovieBooking.Domain.Constants
{
    public class MessageStrings
    {
        public const string BookingCancelled = "Booking cancelled successfully";

        public const string InvalidDate = "Invalid date format. Use YYYY-MM-DD";
        public const string AdminRetrievingError = "An error occurred while retrieving admins";
        public const string InvalidAdminID = "Invalid Admin ID";
        public const string AdminCreteSuccess = "Admin Created Successfully";

        public const string AdminCreationError = "An error occurred while creating admins";
        public const string AdminDeleted = "Admin deleted successfully";
        public const string AdminDeleteError = "An error occured while deleting Admin ";
        public const string AdminUpdated = "Admin updated successfully";

        public const string AdminUpdateError= "An error occured while updating Admin";

        // ================= MOVIE =================

        // Success Messages
        public const string MovieAddedSuccessfully = "Movie added successfully";
        public const string MovieUpdatedSuccessfully = "Movie updated successfully";
        public const string MovieDeletedSuccessfully = "Movie deleted successfully";
        public const string MovieStatusToggledSuccessfully = "Movie status toggled successfully";

        // Error Messages
        public const string ErrorRetrievingMovies = "An error occurred while retrieving movies";
        public const string ErrorRetrievingMovie = "An error occurred while retrieving the movie";
        public const string ErrorAddingMovie = "An error occurred while adding the movie";
        public const string ErrorUpdatingMovie = "An error occurred while updating the movie";
        public const string ErrorDeletingMovie = "An error occurred while deleting the movie";
        public const string ErrorTogglingMovie = "An error occurred while toggling the movie";

        // Validation Messages
        public const string InvalidMovieId = "Invalid movie ID";

        // ================= REQUEST APPROVAL =================

        // Success Messages
        public const string RequestApprovedSuccessfully = "Request approved successfully";
        public const string RequestRejectedSuccessfully = "Request rejected successfully";

        // Error Messages
        public const string ErrorRetrievingRequests = "An error occurred while retrieving requests";
        public const string ErrorRetrievingPendingRequests = "An error occurred while retrieving pending requests";
        public const string ErrorApprovingRequest = "An error occurred while approving the request";
        public const string ErrorRejectingRequest = "An error occurred while rejecting the request";

        // Validation Messages
        public const string InvalidRequestId = "Invalid request ID";

        // ================= SCREEN =================

        // Success Messages
        public const string ScreenAddedSuccessfully = "Screen added successfully";
        public const string ScreenUpdatedSuccessfully = "Screen updated successfully";
        public const string ScreenDeletedSuccessfully = "Screen deleted successfully";

        // Error Messages
        public const string ErrorRetrievingScreens = "An error occurred while retrieving screens";
        public const string ErrorRetrievingScreen = "An error occurred while retrieving the screen";
        public const string ErrorAddingScreen = "An error occurred while adding the screen";
        public const string ErrorUpdatingScreen = "An error occurred while updating the screen";
        public const string ErrorDeletingScreen = "An error occurred while deleting the screen";

        // Validation Messages
        public const string InvalidScreenId = "Invalid screen ID";
        public const string InvalidTheatreId = "Invalid theatre ID";
        public const string InvalidRequest = "Invalid request";

        // ================= SHOWTIME =================

        // Success Messages
        public const string ShowTimesCreatedSuccessfully = "ShowTimes created successfully";
        public const string ShowTimeUpdatedSuccessfully = "ShowTime updated successfully";
        public const string ShowTimeDeletedSuccessfully = "ShowTime deleted successfully";

        // Error Messages
        public const string ErrorRetrievingShowTimes = "An error occurred while retrieving showtimes";
        public const string ErrorRetrievingShowTime = "An error occurred while retrieving the showtime";
        public const string ErrorAddingShowTime = "An error occurred while adding the showtime";
        public const string ErrorUpdatingShowTime = "An error occurred while updating the showtime";
        public const string ErrorDeletingShowTime = "An error occurred while deleting the showtime";

        // Validation Messages
        public const string InvalidShowTimeId = "Invalid showtime ID";

        // ================= THEATRE =================

        // Success Messages
        public const string TheatreAddedSuccessfully = "Theatre added successfully";
        public const string TheatreUpdatedSuccessfully = "Theatre updated successfully";
        public const string TheatreDeletedSuccessfully = "Theatre deleted successfully";

        // Error Messages
        public const string ErrorRetrievingTheatres = "An error occurred while retrieving theatres";
        public const string ErrorRetrievingTheatre = "An error occurred while retrieving the theatre";
        public const string ErrorAddingTheatre = "An error occurred while adding the theatre";
        public const string ErrorUpdatingTheatre = "An error occurred while updating the theatre";
        public const string ErrorDeletingTheatre = "An error occurred while deleting the theatre";

      
    }
}
