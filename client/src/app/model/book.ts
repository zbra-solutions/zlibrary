import { Publisher } from './publisher';
import { Author } from './author';
import { Isbn } from './isbn';
import { Reservation } from './reservation';
import { User } from './user';

export class Book {

    public id: number;
    public title: string;
    public publisher: Publisher;
    public authors: Author[];
    public isbn: string;
    public synopsis: string;
    public publicationYear: number;
    public numberOfCopies: number;
    public coverImageKey: string;
    public created: Date;
    public reservations: Reservation[];

    public get isAvailable(): boolean {
        return this.numberOfCopies > 0 && this.reservations.filter(r => r.reservationReason.isApproved && !!r.loan && !r.loan.isReturned).length < this.numberOfCopies;
    }

    public hasBookReservation(user: User): boolean {
        if (!!this.reservations && this.reservations.length > 0) {
            const userReservations = this.reservations.filter(r => r.userId === user.id);
            return userReservations.length > 0
                && userReservations.some(r => !r.reservationReason.isRejected || r.reservationReason.isApproved && !!r.loan && !r.loan.isReturned);
        }
        return false;
    }
    public calculateExpired(user: User): boolean {
        if (!!this.reservations && this.reservations.length > 0) {
            const userReservations = this.reservations.filter(r => r.userId === user.id);
            return userReservations.length > 0
                && userReservations.some(r => !!r.loan && r.loan.isExpired);
        }
        return false;
    }
}

