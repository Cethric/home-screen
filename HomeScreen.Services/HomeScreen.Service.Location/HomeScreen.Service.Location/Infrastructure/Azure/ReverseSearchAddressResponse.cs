﻿namespace HomeScreen.Service.Location.Infrastructure.Azure;

public class ReverseSearchAddressResponse
{
    public IReadOnlyList<ReverseSearchAddress> Addresses { get; init; } = new List<ReverseSearchAddress>();
}
