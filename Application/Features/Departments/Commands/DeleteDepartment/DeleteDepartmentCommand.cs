using Application.Abstractions.Messaging;

namespace Application.Features.Departments.Commands.DeleteDepartment
{
    public class DeleteDepartmentCommand : ICommand
    {
        public Guid DepartmentId { get; set; }
    }
}
