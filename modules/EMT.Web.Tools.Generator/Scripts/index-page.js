(function ($, ko) {

    var CounterJobModel = function (data) {
        for (var p in data) {
            this[p] = ko.observable(data[p]);
        }
    };

    var IndexPageViewModel = function () {
        var that = this;

        this.counterJobs = ko.observableArray();

        this.loadCounterJobs = function () {
            $.ajax({
                url: "/api/generation/counters/jobs",
                method: "GET"
            })
            .done(function (data) {                
                for (var item in data) {
                    that.counterJobs.push(new CounterJobModel(data[item]));
                }                
            })
            .fail(function () {

            });
        };

        this.convertModelToObject = function (model) {
            var result = {};
            for (var p in model) {
                result[p] = model[p]();
            }
            return result;
        };

        this.onJobStart = function (counterJob) {
            var counterId = counterJob.Id();
            console.log("[JOB START] ", counterId);

            $.ajax({
                url: "/api/generation/counters/jobs/start",
                method: "POST",
                data: that.convertModelToObject(counterJob)
            })
            .done(function (data) {
                console.log("[JOB START] done");
                counterJob.InProgress(true);
            })
            .fail(function () {
                console.log("[JOB START] fail");
            });
        };

        this.onJobStop = function (counterJob) {
            var counterId = counterJob.Id();
            console.log("[JOB STOP] ", counterId);

            $.ajax({
                url: "/api/generation/counters/jobs/stop",
                method: "POST",
                data: that.convertModelToObject(counterJob)
            })
            .done(function (data) {
                console.log("[JOB STOP] done");
                counterJob.InProgress(false);
            })
            .fail(function () {
                console.log("[JOB STOP] fail");
            });
        };

    };


    $(function () {
        var viewModel = new IndexPageViewModel();
        ko.applyBindings(viewModel, $("#index-page").get(0));

        viewModel.loadCounterJobs();
    });

})($, ko);