﻿namespace SurveyBasket.Api.Contracts.Auth
{
    public record ConfirmEmailRequest(
        string UserId,
        string Code
        );

}
