using Application.Abstractions.Messaging;
using Domain.Enums;
using System.Windows.Input;

namespace Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : ICommand<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public UnitType UnitType { get; set; }
    }
}
