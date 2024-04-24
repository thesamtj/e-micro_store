using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Core.Entities;
using Common.Logging.Correlation;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers.V2
{
    public class BasketControllerV2 : ApiControllerV2
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BasketControllerV2> _logger;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;

        public BasketControllerV2(IMediator mediator, IPublishEndpoint publishEndpoint, ILogger<BasketControllerV2> logger, ICorrelationIdGenerator correlationIdGenerator)
        {
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _correlationIdGenerator = correlationIdGenerator;
            _logger.LogInformation("CorrelationId {correlationId}:", _correlationIdGenerator.Get());
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckoutV2 basketCheckout)
        {
            //Get existing basket with username
            var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
            var basket = await _mediator.Send(query);
            if (basket == null)
            {
                return BadRequest();
            }

            var eventMesg = BasketMapper.Mapper.Map<BasketCheckoutEventV2>(basketCheckout);
            eventMesg.TotalPrice = basket.TotalPrice;
            eventMesg.CorrelationId = _correlationIdGenerator.Get();
            await _publishEndpoint.Publish(eventMesg);
            //remove the basket
            var deleteQuery = new DeleteBasketByUserNameQuery(basketCheckout.UserName);
            await _mediator.Send(deleteQuery);
            return Accepted();
        }
    }
}
