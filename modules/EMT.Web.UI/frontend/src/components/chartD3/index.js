import './styles.scss'
import { connect } from 'react-redux'

const w = 1050,
    h = 200,
    padding = 25,
    ticksNumber = 5,
    transitionTime = 1000,
    dotsRadius = 2

class ChartD3 extends React.Component {
    updateChart(data) {
        const now = new Date().getTime(),
            limit = 60 * 1,
            duration = 1000

        const nData = this.normalizeData(data)

        const svg = d3.select(this.refs.chartWithRealTime)

        const minDate = d3.min(nData, d => d.t),
            maxDate = d3.max(nData, d => d.t)

        const xScale = d3.time.scale()
            .domain([new Date(minDate), new Date(maxDate)])
            .range([0, w])

        const yScale = d3.scale.linear()
            .domain([d3.min(nData, d => d.v), d3.max(nData, d => d.v)])
            .range([h - padding, padding])

        const xAxis = d3.svg.axis()
            .scale(xScale)
            .orient('bottom')
            .ticks(ticksNumber)

        const yAxis = d3.svg.axis()
            .scale(yScale)
            .orient('left')
            .ticks(ticksNumber)

        svg.select('.x.axis')
            .transition()
            .duration(transitionTime)
            .call(xAxis)

        svg.select('.y.axis')
            .transition()
            .duration(transitionTime)
            .call(yAxis)

        const chart = svg.select('.chart')

        const line = d3.svg.line()
            .x(d => xScale(d.t))
            .y(d => yScale(d.v))
            .interpolate('linear')

        const chartLine = chart.selectAll('path').data([nData])

        chartLine.attr('d', line)

        chartLine.exit()
            .remove()

        // const chartLine = chart.select('.chart-line')
        //     .data(data)
        //     .attr('d', line)

        // chartLine.data(data)

        //     .attr('d', line)

        // chartLine.data(data).exit().remove()


        // chart.select('.chart-dots')
        //     .selectAll('circle')
        //     .data(data)
        //     .enter()
        //     .append('circle')
        //         .attr('cx', d => xScale(d.t))
        //         .attr('cy', d => yScale(d.v))
        //         .attr('r', dotsRadius)






        // svg.selectAll('text')
        //     .data(data)
        //         .transition()
        //         .duration(durationTransition)
        //             .text(d => d3.round(d.v, 2))
        //             .attr('x', d => xScale(d.t))
        //             .attr('y', d => yScale(d.v))

        // let now = new Date()

        // // Add new values
        // for (var name in this.groups) {
        //     var group = this.groups[name]
        //     //group.data.push(group.value) // Real values arrive at irregular intervals
        //     group.data = this.props.data
        //     group.path.attr('d', this.line)
        // }

        // // Shift domain
        // this.x.domain([now - (this.limit - 2) * this.duration, now - this.duration])

        // // Slide x-axis left
        // this.axis.transition()
        //     .duration(this.duration)
        //     .ease('linear')
        //     .call(this.x.axis)

        // // Slide paths left
        // this.paths.attr('transform', null)
        //     .transition()
        //     .duration(this.duration)
        //     .ease('linear')
        //     .attr('transform', 'translate(' + this.x(now - (this.limit - 1) * this.duration) + ')')
        //     .each('end', this.updateChart)

        // // Remove oldest data point from each group
        // for (var name in this.groups) {
        //     var group = this.groups[name]
        //     group.data.shift()
        // }
    }

    componentWillReceiveProps() {
        this.updateChart(this.props.data)
    }

    normalizeData(data) {
        const rightEdges = []

        let dataset = data.map((d, i) => {
            const nextEl = this.props.data[i + 1],
                interval = 9000

            if (nextEl && ((nextEl.t - d.t) > interval)) {
                rightEdges.push(i)
            }
            return d
        })

        rightEdges.forEach(index => {
            dataset = dataset.concat([
                {
                    t: dataset[index].t + 1,
                    v: 0
                },
                {
                    t: dataset[index + 1].t - 1,
                    v: 0
                }
            ])
        })

        return dataset.sort((a, b) => {
            if (a.t < b.t)
                return -1;
            if (a.t > b.t)
                return 1;
            return 0;
        })
    }

    componentDidMount() {
        const svg = d3.select(this.refs.chartWithRealTime)
            .attr('height', h)
            .attr('width', w)

        const dataset = this.normalizeData(this.props.data)

        const minDate = d3.min(dataset, d => d.t),
            maxDate = d3.max(dataset, d => d.t)

        const xScale = d3.time.scale()
            .domain([new Date(minDate), new Date(maxDate)])
            .range([0, w])

        const yScale = d3.scale.linear()
            .domain([d3.min(dataset, d => d.v), d3.max(dataset, d => d.v)])
            .range([h - padding, padding])

        const xAxis = d3.svg.axis()
            .scale(xScale)
            .orient('bottom')
            .ticks(ticksNumber)

        const yAxis = d3.svg.axis()
            .scale(yScale)
            .orient('left')
            .ticks(ticksNumber)

        svg.append('g')
            .attr('class', 'axis x')
            .attr('transform', `translate(0, ${h - padding})`)
            .call(xAxis)

        svg.append('g')
            .attr('class', 'axis y')
            .attr('transform', `translate(${padding}, 0)`)
            .call(yAxis)

        // Adding visible zone
        svg.append('defs')
            .append('clipPath')
            .attr('id', 'clip')
            .append('rect')
            .attr('width', w - padding)
            .attr('x', padding)
            .attr('y', padding)
            .attr('height', h - padding)

        // Group all elements related to data
        const chart = svg.append('g')
            .attr('class', 'chart')
            .attr('clip-path', 'url(#clip)')

        // Adding line chart based on data coordinates
        const line = d3.svg.line()
            .interpolate('linear')
            .x(d => xScale(d.t))
            .y(d => yScale(d.v))

        chart.selectAll('path')
            .data([dataset])
            .enter()
            .append('path')
            .attr('d', line)
            .attr('class', 'chart-line')
            .attr('transform', null)

        chart.append('line')
            .attr('class', 'chart-line-active')
            .attr('fill', 'red')
            .attr('x1', 0)
            .attr('y1', 0)
            .attr('x2', 0)
            .attr('y2', h - padding)
            .attr('stroke', 'red')

        // Add current position line
        const svgTarget = d3.selectAll('.chart-real-time svg'),
            lineCurrentTime = svgTarget.selectAll('.chart-line-active')

        svgTarget.on('mousemove', null)
        svgTarget.on('mousemove', function () {
            const coordinates = d3.mouse(this)

            lineCurrentTime.attr('x1', coordinates[0])
                .attr('x2', coordinates[0])
        })



        // Adding dots to the chart
        // chart.append('g')
        //     .attr('class', 'chart-dots')
        //         .selectAll('circle')
        //         .data(dataset)
        //         .enter()
        //         .append('circle')
        //             .attr('cx', d => xScale(d.t))
        //             .attr('cy', d => yScale(d.v))
        //             .attr('r', dotsRadius)








        // svg.selectAll('text')
        //     .data(dataset)
        //     .enter()
        //     .append('text')
        //         .text(d => d3.round(d.v, 2))
        //         .attr('x', d => xScale(d.t))
        //         .attr('y', d => yScale(d.v))
        //         .attr('class', 'point-label')


        // const random = () => Math.random()*1,
        //     data = []

        // data.push(random())

        // // redraw the line, and then slide it to the left
        // d3.select(this.refs.chartWithRealTime)
        //     .append('path')
        //         .attr("d", line)
        //         .attr("transform", null)
        //     .transition()
        //         .attr("transform", "translate(" + x(-1) + ")");

        // // pop the old data point off the front
        // data.shift();


        // this.limit = 60 * 1
        // this.duration = 1000

        // var now = new Date(Date.now() - this.duration)

        // var width = 1000,
        //     height = 200

        // this.groups = {
        //     // current: {
        //     //     value: 0,
        //     //     color: 'orange',
        //     //     data: d3.range(limit).map(() => 0)
        //     // },
        //     target: {
        //         value: 0,
        //         color: 'green',
        //         // data: d3.range(limit).map(() => 0)
        //         data: this.props.data.map(el => el.v)
        //     },
        //     // output: {
        //     //     value: 0,
        //     //     color: 'grey',
        //     //     data: d3.range(limit).map(() => 0)
        //     // }
        // }

        // this.x = d3.time.scale()
        //     .domain([now - (this.limit - 2), now - this.duration])
        //     .range([0, width])

        // this.y = d3.scale.linear()
        //     .domain([0, 1])
        //     .range([height, 0])

        // this.line = d3.svg.line()
        //     .interpolate('basis')
        //     .x((d, i) => this.x(now - (this.limit - 1 - i) * this.duration))
        //     .y(d => this.y(d))

        // this.svg = d3.select(this.refs.chartWithRealTime)
        //     .attr('class', 'chart')
        //     .attr('width', width)
        //     .attr('height', height + 50)

        // this.axis = this.svg.append('g')
        //     .attr('class', 'x axis')
        //     .attr('transform', 'translate(0,' + height + ')')
        //     .call(this.x.axis = d3.svg.axis().scale(this.x).orient('bottom'))

        // var paths = this.svg.append('g')

        // for (var name in this.groups) {
        //     var group = this.groups[name]
        //     group.path = paths.append('path')
        //         .data([group.data])
        //         .attr('class', name + ' group')
        //         .style('stroke', group.color)
        // }

        // this.updateChart()
    }

    render() {
        return (
            <div className="chart-real-time">
                <svg ref="chartWithRealTime"></svg>
            </div>
        )
    }
}

export default ChartD3
