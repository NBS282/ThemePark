import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TicketService } from '../../../../core/services/ticket.service';

@Component({
  selector: 'app-my-tickets-page',
  imports: [CommonModule, RouterLink],
  templateUrl: './my-tickets-page.component.html',
  styleUrl: './my-tickets-page.component.css'
})
export class MyTicketsPageComponent implements OnInit {
  constructor(public ticketService: TicketService) {}

  ngOnInit(): void {
    this.ticketService.getMyTickets();
  }

  get tickets() {
    return this.ticketService.myTickets();
  }

  get isLoading() {
    return this.ticketService.isLoading();
  }

  get error() {
    return this.ticketService.error();
  }
}
