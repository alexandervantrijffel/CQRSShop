﻿namespace CQRSShop.Contracts.Commands
open CQRSShop.Infrastructure
open System

// Customer commands
type CreateCustomer = {Id: Guid; Name: string } with interface ICommand
type MarkCustomerAsPreferred = {Id: Guid; Discount: int } with interface ICommand

// Product commands
type CreateProduct = {Id: Guid; Name: string; Price: int } with interface ICommand