using System;

namespace ZLibrary.Model
{
    public class Reservation
    {
        public long Id { get; set; }
        public User User { get; private set; }
        public long BookId { get; private set; }
        public ReservationReason Reason { get; private set; }
        public DateTime StartDate { get; private set; }

        public bool IsRequested => Reason.Status == ReservationStatus.Requested;
        public bool IsApproved => Reason.Status == ReservationStatus.Approved;
        public bool IsRejected =>  Reason.Status == ReservationStatus.Rejected;
        
        public Reservation(long bookId, User user)
        {
            if (bookId <= 0)
            {
                throw new ArgumentNullException($"The paramenter {nameof(bookId)} must be greater than zero.");
            }
            if (user == null)
            {
                throw new ArgumentNullException($"The paramenter {nameof(user)} can not be null.");
            }

            User = user;
            BookId = bookId;
            StartDate = DateTime.Now;
            Reason = new ReservationReason();
        }

        private Reservation()
        {
        }

       
    }
}