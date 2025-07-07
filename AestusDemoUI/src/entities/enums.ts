export enum Controller {
  DashboardController = 'dashboard',
}

export enum HttpResponseStatusEnum {
  UnknownError = 0,
  BadRequest = 400,
  NotAuthorized = 401,
  NotFound = 404,
  InternalServerError = 500,
}
