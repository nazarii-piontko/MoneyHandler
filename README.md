Money Handler
===================

.NET class library for money handling. It is advanced implementation of Enterprise Pattern Money described by Martin Fowler for .NET.

###Posibilities:
* Create (instantiate) 'money', see example #1. 
* Convert money from one currency to another (using different currencies rates providers, e.g. Yahoo Finance), example #2.
* Perform different arifmetic and boolean operations with automatic currency convertation if need, example #3.
* Support Binary and XML serialization.
* Parsing from String.
* etc.

##Usage

###Example #1 - Instantiating

* <code>1m.Euros();</code>
* <code>new Money(1, Currency.USD);</code>
* <code>Money.Parse("18 EUR");</code>

###Example #2 - Converting

* <code>1m.Euros().ToDollars();</code>
* <code>1m.Euros().ConvertToCurrency(Currency.GBP);</code>

###Example #3 - Operations

* <code>2m.Dollars() + 3m.Euros();</code>
* <code>2m.Dollars() + 2;</code>
* <code>2m.Dollars() <= 2m.Euros();</code>
