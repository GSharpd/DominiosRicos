using Flunt.Notifications;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Handlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : Notifiable, IHandler<CreateBoletoSubscriptionCommand>, IHandler<CreatePayPalSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        private readonly IEmailService _emailService;

        public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível realizar sua assinatura");
            }

            if (_repository.DocumentExists(command.Document))
                AddNotification("Document", "CPF em uso");

            if (_repository.EmailExists(command.Email))
                AddNotification("Email", "Email em uso");

            var name = new Name(command.FirstName, command.LastName);
            var doc = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            var student = new Student(name, doc, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(
                command.BarCode, 
                command.BoletoNumber, 
                command.PaidDate, 
                command.ExpireDate, 
                command.Total, 
                command.TotalPaid, 
                command.Payer,
                new Document(command.PayerDocument, command.PayerDocumentType), 
                address, 
                email);

            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            AddNotifications(name, doc, email, address, student, subscription, payment);

            if (Invalid)
                return new CommandResult(false, "Não foi possível realizar sua assinatura");

            _repository.CreateSubscription(student);
            _emailService.Send(student.Name.ToString(), student.Email.Address, "bem vindo ao mad.io", "all you base are belong to us");

            return new CommandResult(true, "Assinatura realizada com sucesso");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
            if (_repository.DocumentExists(command.Document))
                AddNotification("Document", "CPF em uso");

            if (_repository.EmailExists(command.Email))
                AddNotification("Email", "Email em uso");

            var name = new Name(command.FirstName, command.LastName);
            var doc = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            var student = new Student(name, doc, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(
                command.TransactionCode,
                command.PaidDate,
                command.ExpireDate,
                command.Total,
                command.TotalPaid,
                command.Payer,
                new Document(command.PayerDocument, command.PayerDocumentType),
                address,
                email);

            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            AddNotifications(name, doc, email, address, student, subscription, payment);

            if (Invalid)
                return new CommandResult(false, "Não foi possível realizar sua assinatura");

            _repository.CreateSubscription(student);
            _emailService.Send(student.Name.ToString(), student.Email.Address, "bem vindo ao mad.io", "all you base are belong to us");

            return new CommandResult(true, "Assinatura realizada com sucesso");
        }
    }
}
