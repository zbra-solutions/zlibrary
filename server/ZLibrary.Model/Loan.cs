using System;

namespace ZLibrary.Model
{
    public class Loan
    {
        public Reservation Reservation { get; private set; }
        public Date ExpirationDate { get; private set; }
        public LoanStatus Status { get; set; }

        public Loan(Reservation reservation)
        {
            if (reservation == null)
            {
                throw new ArgumentNullException($"The paramenter {nameof(reservation)} can not be null.");
            }

            Reservation = reservation;
            Status = LoanStatus.Borrowed;

            CalculateExpirationDate();
        }
        private void CalculateExpirationDate()
        {
            var dateNow = DateTime.Now.Date;
            ExpirationDate = new Date(dateNow.Add(new TimeSpan(90, 0, 0, 0)));
        }
    }
}