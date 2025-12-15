import { Routes } from '@angular/router';
import { LoginPageComponent } from './features/auth/pages/login-page/login-page.component';
import { RegisterPageComponent } from './features/auth/pages/register-page/register-page.component';
import { ProfilePageComponent } from './features/profile/pages/profile-page/profile-page.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { HomePageComponent } from './features/home/pages/home-page/home-page.component';
import { UsersListComponent } from './features/admin/pages/users-list/users-list.component';
import { UserDetailComponent } from './features/admin/pages/user-detail/user-detail.component';
import { NotFoundPageComponent } from './features/error/pages/not-found-page/not-found-page.component';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';
import { operatorGuard } from './core/guards/operator.guard';
import { AttractionsListComponent } from './features/attractions/pages/attractions-list/attractions-list.component';
import { AttractionDetailComponent } from './features/attractions/pages/attraction-detail/attraction-detail.component';
import { UsageReportComponent } from './features/attractions/pages/usage-report/usage-report.component';
import { OperatorDashboardComponent } from './features/attractions/pages/operator-dashboard/operator-dashboard.component';
import { OperatorControlComponent } from './features/attractions/pages/operator-control/operator-control.component';
import { PublicAttractionsComponent } from './features/attractions/pages/public-attractions/public-attractions.component';
import { MaintenanceManagementComponent } from './features/attractions/pages/maintenance-management/maintenance-management.component';
import { RewardsListComponent } from './features/rewards/pages/rewards-list/rewards-list.component';
import { RewardsAdminComponent } from './features/rewards/pages/rewards-admin/rewards-admin.component';
import { RewardDetailComponent } from './features/rewards/pages/reward-detail/reward-detail.component';
import { ExchangeHistoryComponent } from './features/rewards/pages/exchange-history/exchange-history.component';
import { SystemDatetimeConfigComponent } from './features/admin/pages/system-datetime-config/system-datetime-config.component';
import { PurchaseTicketPageComponent } from './features/tickets/pages/purchase-ticket-page/purchase-ticket-page.component';
import { MyTicketsPageComponent } from './features/tickets/pages/my-tickets-page/my-tickets-page.component';
import { ScoringStrategiesAdminComponent } from './features/scoring-strategies/pages/scoring-strategies-admin/scoring-strategies-admin.component';
import { EventsListComponent } from './features/events/pages/events-list/events-list.component';
import { EventDetailComponent } from './features/events/pages/event-detail/event-detail.component';
import { PublicEventsComponent } from './features/events/pages/public-events/public-events.component';
import { RankingPageComponent } from './features/scoring/pages/ranking-page/ranking-page.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', component: HomePageComponent },
      { path: 'login', component: LoginPageComponent },
      { path: 'register', component: RegisterPageComponent },
      { path: 'profile', component: ProfilePageComponent, canActivate: [authGuard] },
      { path: 'admin/users', component: UsersListComponent, canActivate: [authGuard, adminGuard] },
      { path: 'admin/users/:id', component: UserDetailComponent, canActivate: [authGuard, adminGuard] },
      { path: 'admin/system-datetime', component: SystemDatetimeConfigComponent, canActivate: [authGuard, adminGuard] },
      { path: 'scoring/ranking', component: RankingPageComponent, canActivate: [authGuard, adminGuard] },
      { path: 'rewards', component: RewardsListComponent, canActivate: [authGuard] },
      { path: 'rewards/admin', component: RewardsAdminComponent, canActivate: [authGuard, adminGuard] },
      { path: 'rewards/history', component: ExchangeHistoryComponent, canActivate: [authGuard] },
      { path: 'rewards/:id', component: RewardDetailComponent, canActivate: [authGuard] },
      { path: 'admin/users/:id', component: UserDetailComponent, canActivate: [authGuard, adminGuard] },

      { path: 'attractions/manage', component: AttractionsListComponent, canActivate: [authGuard, adminGuard] },
      { path: 'attractions/manage/new', component: AttractionDetailComponent, canActivate: [authGuard, adminGuard] },
      { path: 'attractions/manage/:nombre', component: AttractionDetailComponent, canActivate: [authGuard, adminGuard] },
      { path: 'attractions/report', component: UsageReportComponent, canActivate: [authGuard, adminGuard] },

      { path: 'attractions/operator', component: OperatorDashboardComponent, canActivate: [authGuard, operatorGuard] },
      { path: 'attractions/operator/:nombre', component: OperatorControlComponent, canActivate: [authGuard, operatorGuard] },
      { path: 'attractions/operator/:nombre/maintenance', component: MaintenanceManagementComponent, canActivate: [authGuard, operatorGuard] },

      { path: 'attractions', component: PublicAttractionsComponent, canActivate: [authGuard] },

      { path: 'tickets/purchase', component: PurchaseTicketPageComponent, canActivate: [authGuard] },
      { path: 'tickets/my-tickets', component: MyTicketsPageComponent, canActivate: [authGuard] },

      { path: 'admin/scoring-strategies', component: ScoringStrategiesAdminComponent, canActivate: [authGuard, adminGuard] },

      { path: 'events/manage', component: EventsListComponent, canActivate: [authGuard, adminGuard] },
      { path: 'events/manage/new', component: EventDetailComponent, canActivate: [authGuard, adminGuard] },
      { path: 'events/manage/:id', component: EventDetailComponent, canActivate: [authGuard, adminGuard] },

      { path: 'events', component: PublicEventsComponent, canActivate: [authGuard] }
    ]
  },
  { path: '**', component: NotFoundPageComponent }
];
