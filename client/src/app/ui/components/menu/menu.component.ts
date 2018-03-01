import {Component, OnInit, ViewEncapsulation} from '@angular/core';
import { ReservationHistoryComponent } from '../reservation-history/reservation-history.component';
//import { WaitingListComponent } from '../waiting-list/waiting-list.component';
import { BsModalService } from 'ngx-bootstrap';
import { AuthService } from '../../../service/auth.service';
import { User } from '../../../model/user';
import { BsModalRef } from 'ngx-bootstrap/modal/bs-modal-ref.service';

@Component({
    selector: 'zli-menu',
    templateUrl: './menu.component.html',
    styleUrls: ['./menu.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class MenuComponent implements OnInit {

    public user: User;
    public showUserMenu: boolean;

    constructor(private modalService: BsModalService,
        private service: AuthService) {
    }
    
    ngOnInit() {
        this.user = this.service.getLoggedUser();
    }

    public viewReservationHistory(): void {
        this.modalService.show(ReservationHistoryComponent);
    }

    /*public viewWaitingList(): void {
        this.modalService.show(WaitingListComponent);
    }*/

    public onLogout() {
        this.service.logout();
        window.location.reload();
    }

    public toggleMenu() {
        this.showUserMenu = !this.showUserMenu;
    }

    public showRentedBooks(){
        let reservationHistoryModalControl = this.modalService.show(ReservationHistoryComponent)
        let reservationHistoryComponent = reservationHistoryModalControl.content as ReservationHistoryComponent;
        reservationHistoryComponent.modalControl = reservationHistoryModalControl;
    }

    /*public showWaitingList(){
        let waitingListModalControl = this.modalService.show(WaitingListComponent)
        let waitingListModalComponent = waitingListModalControl.content as WaitingListComponent;
        waitingListModalComponent.modalControl = waitingListModalControl;
    }*/
}
