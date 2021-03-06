using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZLibrary.API;
using ZLibrary.Model;
using System.Collections.Generic;
using System.Linq;
using ZLibrary.Web.Controllers.Items;
using ZLibrary.Web.Validators;
using ZLibrary.Web.Extensions;
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using ZLibrary.Web.LookUps;

namespace ZLibrary.Web
{
    [Route("api/[controller]")]
    public class ReservationsController : Controller
    {
        private readonly IReservationService reservationService;
        private readonly IUserService userService;
        private readonly IBookService bookService;
        private readonly ILoanService loanService;
        private readonly IServiceDataLookUp serviceDataLookUp;

        public ReservationsController(IReservationService reservationService, IUserService userService, IBookService bookService, ILoanService loanService)
        {
            this.reservationService = reservationService;
            this.userService = userService;
            this.bookService = bookService;
            this.loanService = loanService;
            this.serviceDataLookUp = new DefaultServiceDataLookUp(loanService, reservationService);
        }

        [HttpGet]
        public async Task<IActionResult> FindAll()
        {
            var reservations = await reservationService.FindAll();
            return Ok(await reservations.ToReservationViewItems(serviceDataLookUp));
        }

        [HttpGet("{id:long}", Name = "FindReservation")]
        public async Task<IActionResult> FindById(long id)
        {
            var reservation = await reservationService.FindById(id);
            if (reservation == null)
            {
                return NotFound($"Nenhuma reserva encontrada com o ID: {id}.");
            }
            return Ok(await reservation.ToReservationViewItem(serviceDataLookUp));
        }

        [HttpGet("user/{userId:long}", Name = "FindReservationByUserId")]
        public async Task<IActionResult> FindByUserId(long userId)
        {
            var reservations = await reservationService.FindByUserId(userId);
            if (reservations == null || !reservations.Any())
            {
                return NotFound($"Nenhuma reserva encontrada com o ID do usuário: {userId}.");
            }
            return Ok(await reservations.ToReservationViewItems(serviceDataLookUp));
        }

        [HttpGet("status/{statusName}", Name = "FindReservationsByStatus")]
        public async Task<IActionResult> FindReservationsByStatus(string statusName)
        {
            ReservationStatus reservationStatus;
            if (!Enum.TryParse<ReservationStatus>(statusName, out reservationStatus)) 
            {
                return BadRequest($"Não foi possível converter o status [{statusName}]");
            }
            var reservations = await reservationService.FindByStatus(reservationStatus);
            if (reservations == null || !reservations.Any())
            {
                return NotFound($"Nenhuma reserva encontrada com o status: {reservationStatus}.");
            }
            return Ok(await reservations.ToReservationViewItems(serviceDataLookUp));
        }

        [HttpGet("orders/{statusName}", Name = "FindOrdersByStatus")]
        public async Task<IActionResult> FindOrdersByStatus(string statusName)
        {
            ReservationStatus reservationStatus;
            if (!Enum.TryParse<ReservationStatus>(statusName, out reservationStatus)) 
            {
                return BadRequest($"Não foi possível converter o status [{statusName}]");
            }
            var reservations = await reservationService.FindByStatus(reservationStatus);
            if (reservations == null || !reservations.Any())
            {
                return NotFound($"Nenhuma reserva encontrada com o status: {reservationStatus}.");
            }
            var orderList = new List<OrderDTO>();
            var reservationResultList = await reservations.ToReservationViewItems(serviceDataLookUp);
            foreach (var reservationResult in reservationResultList) 
            {
                var order = new OrderDTO();
                order.Reservation = reservationResult;
                order.Book = await (await bookService.FindById(reservationResult.BookId)).ToBookViewItem(serviceDataLookUp);
                order.User = (await userService.FindById(reservationResult.UserId)).ToUserViewItem();
                orderList.Add(order);
            }
            return Ok(orderList);
        }

        [HttpPost("order")]
        public async Task<IActionResult> Order([FromBody]ReservationRequestDTO value)
        {
            var user = await userService.FindById(value.UserId);
            if (user == null)
            {
                return NotFound($"Nenhum usuário encontrado com o ID: {value.UserId}.");
            }

            var book = await bookService.FindById(value.BookId);
            if (book == null)
            {
                return NotFound($"Nenhum livro encontrado com o ID: {value.BookId}.");
            }

            var reservation = await reservationService.Order(book, user);
            return Ok(reservation.ToReservationViewItem());
        }

        [HttpPost("approved/{id:long}")]
        public async Task<IActionResult> UpdateLoanStatusToApproved(long id)
        {
            var reservation = await reservationService.FindById(id);
            if (reservation == null)
            {
                return NotFound($"Nenhuma reserva encontrada com o ID: {id}.");
            }
            try
            {
                var book = await bookService.FindById(reservation.BookId);
                if (book == null)
                {
                    return NotFound($"Nenhum livro encontrado com o ID: {reservation.BookId}.");
                }
                await reservationService.ApprovedReservation(reservation, book);
                return Ok();
            }
            catch (ReservationApprovedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("rejected")]
        public async Task<IActionResult> UpdateLoanStatusToRejected([FromBody]ReservationRejectDTO value)
        {
            var reservation = await reservationService.FindById(value.Id);
            if (reservation == null)
            {
                return NotFound($"Nenhuma reserva encontrada com o ID: {value.Id}.");
            }
            reservation.Reason.Description = value.Description;
            try
            {
                await reservationService.RejectedReservation(reservation);
                return Ok();
            }
            catch (ReservationApprovedException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
