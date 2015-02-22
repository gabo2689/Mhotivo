﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Mhotivo.Data.Entities;
using Mhotivo.Interface.Interfaces;
using Mhotivo.Logic.ViewMessage;
using Mhotivo.Models;
using Microsoft.Ajax.Utilities;

namespace Mhotivo.Controllers
{
    public class AcademicYearDetailsController : Controller
    {
        private readonly IAcademicYearDetailsRepository _academicYearDetailsRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMeisterRepository _meisterRepository;
        private readonly ViewMessageLogic _viewMessageLogic;

        public AcademicYearDetailsController(IAcademicYearDetailsRepository academicYearDetailsRepository,
            ICourseRepository courseRepository, IMeisterRepository meisterRepository)
        {
            _academicYearDetailsRepository = academicYearDetailsRepository;
            _courseRepository = courseRepository;
            _meisterRepository = meisterRepository;
            _viewMessageLogic = new ViewMessageLogic(this);
        }

        public ActionResult Index(int id)
        {
            _viewMessageLogic.SetViewMessageIfExist();
            var allAcademicYears = _academicYearDetailsRepository.GetAllAcademicYearsDetails(id);
            var academicYearsDetails = allAcademicYears.Select(academicYearD => academicYearD.Schedule != null ? (academicYearD.TeacherEndDate != null ? (academicYearD.TeacherStartDate != null ? new DisplayAcademicYearDetailsModel
            {
                Id = academicYearD.Id,
                TeacherStartDate = academicYearD.TeacherStartDate.Value,
                TeacherEndDate = academicYearD.TeacherEndDate.Value,
                Schedule = academicYearD.Schedule.Value,
                Room = academicYearD.Room,
                Course = academicYearD.Course.Name,
                Teacher = academicYearD.Teacher.FullName
            } : null) : null) : null).ToList();

            return View(academicYearsDetails);
        }

        [HttpGet]
        public ActionResult AcademicYearDetailsEdit(int id)
        {
            return null;
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var academicYearDetails = _academicYearDetailsRepository.GetById(id);
            var academicYearModel = new AcademicYearDetailsEditModel
            {
                Id = academicYearDetails.Id,
                TeacherStartDate = academicYearDetails.TeacherStartDate.ToString(),
                TeacherEndDate = academicYearDetails.TeacherEndDate.ToString(),
                Schedule = academicYearDetails.Schedule.ToString(),
                Room = academicYearDetails.Room,
                Course = academicYearDetails.Course,
                Teacher = academicYearDetails.Teacher
            };

            ViewBag.CourseId = new SelectList(_courseRepository.Query(x => x), "Id", "Name", academicYearModel.Course.Id);
            ViewBag.MeisterId = new SelectList(_meisterRepository.Query(x => x), "Id", "FullName", academicYearModel.Teacher.Id);

            return View("Edit", academicYearModel);
        }

        [HttpPost]
        public ActionResult Edit(AcademicYearDetailsEditModel academicYearDetailsModel)
        {
            var myAcademicYearDetails = _academicYearDetailsRepository.GetById(academicYearDetailsModel.Id);
            myAcademicYearDetails.TeacherStartDate = academicYearDetailsModel.TeacherStartDate.AsDateTime();
            myAcademicYearDetails.TeacherEndDate = academicYearDetailsModel.TeacherEndDate.AsDateTime();
            myAcademicYearDetails.Schedule = academicYearDetailsModel.Schedule.AsDateTime();
            myAcademicYearDetails.Room = academicYearDetailsModel.Room;
            myAcademicYearDetails.Course = _courseRepository.GetById(academicYearDetailsModel.Course.Id);
            myAcademicYearDetails.Teacher = _meisterRepository.GetById(academicYearDetailsModel.Teacher.Id);

            _academicYearDetailsRepository.Update(myAcademicYearDetails);

            const string title = "El Detalle del Año Académico Actualizado ";
            var content = "El detalle " + myAcademicYearDetails.Course.Name + " ha sido actualizado exitosamente.";
            _viewMessageLogic.SetNewMessage(title, content, ViewMessageType.InformationMessage);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var academicYearDetail = _academicYearDetailsRepository.Delete(id);

            const string title = "Detalle Académico Eliminado";
            var content = "El detalle de año académico " + academicYearDetail.Course.Name + " ha sido eliminado exitosamente.";
            _viewMessageLogic.SetNewMessage(title, content, ViewMessageType.InformationMessage);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.CourseId = new SelectList(_courseRepository.Query(x => x), "Id", "Name", 0);
            ViewBag.MeisterId = new SelectList(_meisterRepository.Query(x => x), "Id", "FullName", 0);

            return View("Create");
        }

        [HttpPost]
        public ActionResult Add(AcademicYearDetailsRegisterModel academicYearDetailsModel)
        {
            var academicYearDetails = new AcademicYearDetail
            {
                TeacherStartDate = Convert.ToDateTime(academicYearDetailsModel.TeacherStartDate),
                TeacherEndDate = Convert.ToDateTime(academicYearDetailsModel.TeacherEndDate),
                Schedule = Convert.ToDateTime(academicYearDetailsModel.Schedule),
                Room = academicYearDetailsModel.Room,
                Course = _courseRepository.GetById(academicYearDetailsModel.Course.Id),
                Teacher = _meisterRepository.GetById(academicYearDetailsModel.Teacher.Id)
            };

            var academicY = _academicYearDetailsRepository.Create(academicYearDetails);
            const string title = "Detalles de Año Académico Agregado";
            const string content = "El detalle del año académico ha sido agregado exitosamente.";
            _viewMessageLogic.SetNewMessage(title, content, ViewMessageType.SuccessMessage);

            return RedirectToAction("Index");
        }
    }
}