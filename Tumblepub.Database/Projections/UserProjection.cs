﻿using Marten.Events.Aggregation;
using Tumblepub.Database.Events;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Projections;

public class UserProjection : AggregateProjection<User>
{
    public UserProjection()
    {
        DeleteEvent<UserDeleted>();
    }

    public User Create(UserCreated e)
    {
        return new User
        {
            Id = e.UserId,
            Email = e.Email,
            PasswordHash = e.PasswordHash,
            UpdatedAt = e.At,
            CreatedAt = e.At
        };
    }
}