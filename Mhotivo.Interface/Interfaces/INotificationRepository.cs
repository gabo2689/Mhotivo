﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mhotivo.Data.Entities;

namespace Mhotivo.Interface.Interfaces
{
    public interface INotificationRepository
    {
        Notification First(Expression<Func<Notification, bool>> query);
        Notification GetById(long id);
        Notification Create(Notification itemToCreate);
        IQueryable<Notification> Query(Expression<Func<Notification, Notification>> expression);
        IQueryable<Notification> Where(Expression<Func<Notification, bool>> expression);
        Notification Update(Notification itemToUpdate);
        Notification Delete(Notification itemToDelete);
        Notification Delete(long id);
        IEnumerable<Notification> GetAllNotifications();
    }
}
