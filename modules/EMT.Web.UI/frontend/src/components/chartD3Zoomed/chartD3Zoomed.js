import './chartD3Zoomed.scss'

const W = 1050,
    H = 300,

    //Setting up Margins
    mainMargin = { top: 10, right: 25, left: 25, bottom: 140 },
    subMargin = { top: 200, right: 25, bottom: 40, left: 25 },

    //Widths, Heights
    width = W - mainMargin.left - mainMargin.right,
    mainHeight = H - mainMargin.top - mainMargin.bottom,
    subHeight = H - subMargin.top - subMargin.bottom,
    isValidDate = date => date instanceof Date && !isNaN(date.valueOf())

let brush = d3.svg.brush()

window.brushExtent = false

class ChartD3Zoomed extends React.Component {
    constructor(props) {
        super(props)
        this.normalizeData = this.normalizeData.bind(this)
    }

    normalizeData(data) {
        const rightEdges = [],
            dateFrom = this.props.dateFrom,
            dateTo = this.props.dateTo

        let dataset = data.map((d, i) => {
            const nextEl = this.props.data[i + 1],
                interval = 9000

            if (nextEl && ((nextEl.t - d.t) > interval)) {
                rightEdges.push(i)
            }
            return d
        })

        if (dataset.length) {
            dataset = dataset.concat([
                {
                    t: new Date(dateFrom).getTime(),
                    v: 0,
                    fake: true
                },
                {
                    t: dataset[0].t - 1,
                    v: 0,
                    fake: true
                },
                {
                    t: dataset[dataset.length - 1].t + 1,
                    v: 0,
                    fake: true
                },
                {
                    t: new Date(dateTo).getTime(),
                    v: 0,
                    fake: true
                }
            ])
        }

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

    componentDidUpdate() {
        const data = this.normalizeData(this.props.data)

        const dataExtent = d3.extent(data, d => d.t)
        //Main Chart Scales
        const mainXScale = d3.time.scale()
            .domain(dataExtent)
            .range([0, width])

        const mainYScale = d3.scale.linear()
            .domain([0, d3.max(data, d => d.v)])
            .range([mainHeight, 0])

        //Sub Chart scales
        const subXScale = d3.time.scale()
            .domain(dataExtent)
            .range([0, width])

        const subYScale = d3.scale.linear()
            .domain(mainYScale.domain())
            .range([subHeight, 0])

        // Update brush
        // brush.x(subXScale)

        //Main Chart Axes
        const mainXAxis = d3.svg.axis().scale(mainXScale).orient('bottom')
        const mainYAxis = d3.svg.axis().scale(mainYScale).orient('left')

        //Sub Chart Axes
        const subXAxis = d3.svg.axis().scale(subXScale).orient('bottom')
        const subYAxis = d3.svg.axis().scale(subYScale).orient('left').ticks(2)

        //Area
        const mainArea = d3.svg.area()
            .interpolate('linear')
            .x(d => mainXScale(d.t))
            .y0(mainHeight)
            .y1(d => mainYScale(d.v))

        const subArea = d3.svg.area()
            .interpolate('linear')
            .x(d => subXScale(d.t))
            .y0(subHeight)
            .y1(d => subYScale(d.v))

        const svg = d3.select(this.refs.chartWithZoom)

        const main = svg.select('.main')
            .attr('transform', `translate(${mainMargin.left}, ${mainMargin.top})`)

        const sub = svg.select('.sub')
            .attr('transform', `translate(${subMargin.left}, ${subMargin.top})`)

        mainXScale.domain(brush.empty() ? subXScale.domain() : brush.extent())

        if (window.brushExtent && isValidDate(window.brushExtent[0])) {
            brush.extent(window.brushExtent)
            brush(d3.selectAll('.brush'))
            brush.event(d3.selectAll('.x.brush'))
        }

        main.select('.x.axis')
            .attr('transform', `translate(0, ${mainHeight})`)
            .call(mainXAxis)

        main.select('.y.axis')
            .attr('transform', 'translate(0, 0)')
            .call(mainYAxis)

        sub.select('.x.axis')
            .attr('transform', `translate(0, ${subHeight})`)
            .call(subXAxis)

        sub.select('.y.axis')
            .attr('transform', 'translate(0, 0)')
            .call(subYAxis)

        main.select('.area')
            .datum(data)
            .attr('d', mainArea)

        sub.select('.area')
            .datum(data)
            .attr('d', subArea)
    }

    componentDidMount() {
        const bisectDate = d3.bisector(d => d.t).left,
            data = this.normalizeData(this.props.data)

        const dataExtent = d3.extent(data, d => d.t)

        //Main Chart Scales
        const mainXScale = d3.time.scale()
            .domain(dataExtent)
            .range([0, width])

        const mainYScale = d3.scale.linear()
            .domain([0, d3.max(data, d => d.v)])
            .range([mainHeight, 0])

        //Sub Chart scales
        const subXScale = d3.time.scale()
            .domain(dataExtent)
            .range([0, width])

        const subYScale = d3.scale.linear()
            .domain(mainYScale.domain())
            .range([subHeight, 0])

        //Main Chart Axes
        const mainXAxis = d3.svg.axis().scale(mainXScale).orient('bottom'),
            mainYAxis = d3.svg.axis().scale(mainYScale).orient('left')

        //Sub Chart Axes
        const subXAxis = d3.svg.axis().scale(subXScale).orient('bottom'),
            subYAxis = d3.svg.axis().scale(subYScale).orient('left').ticks(2)

        //Area
        const mainArea = d3.svg.area()
            .interpolate('linear')
            .x(d => mainXScale(d.t))
            .y0(mainHeight)
            .y1(d => mainYScale(d.v))

        const subArea = d3.svg.area()
            .interpolate('linear')
            .x(d => subXScale(d.t))
            .y0(subHeight)
            .y1(d => subYScale(d.v))

        const svg = d3.select(this.refs.chartWithZoom)
            .attr('width', W)
            .attr('height', H)

        svg.append('defs')
            .append('clipPath')
            .attr('id', 'clip')
            .append('rect')
            .attr('width', width)
            .attr('height', mainHeight)

        const main = svg.append('g')
            .classed('main', true)
            .attr('transform', `translate(${mainMargin.left}, ${mainMargin.top})`)

        const sub = svg.append('g')
            .classed('sub', true)
            .attr('transform', `translate(${subMargin.left}, ${subMargin.top})`)

        brush.x(subXScale)
            .on('brush', brushed)
            .on('brushend', function () {
                // brush.extent(window.brushExtent)
                // brush(d3.selectAll('.brush'))
            })

        main.append('g')
            .classed('x axis', true)
            .attr('transform', `translate(0, ${mainHeight})`)
            .call(mainXAxis)

        main.append('g')
            .classed('y axis', true)
            .attr('transform', 'translate(0, 0)')
            .call(mainYAxis)

        main.append('path')
            .datum(data)
            .classed('area', true)
            .attr('d', mainArea)

        sub.append('g')
            .classed('x axis', true)
            .attr('transform', `translate(0, ${subHeight})`)
            .call(subXAxis)

        sub.append('g')
            .classed('y axis', true)
            .attr('transform', 'translate(0, 0)')
            .call(subYAxis)

        sub.append('path')
            .datum(data)
            .classed('area', true)
            .attr('d', subArea)

        sub.append('g')
            .classed('x brush', true)
            .call(brush)
            .selectAll('rect')
            .attr('y', -6)
            .attr('height', subHeight + 7)

        if (window.brushExtent && isValidDate(window.brushExtent[0])) {
            brush.extent(window.brushExtent)
            brush(d3.selectAll('.brush'))
            brush.event(d3.selectAll('.x.brush'))
        }

        function brushed(e) {
            window.brushExtent = brush.empty() ? subXScale.domain() : brush.extent()
            if (!isValidDate(window.brushExtent[0])) return

            mainXScale.domain(window.brushExtent)

            const allMain = d3.selectAll('.chart-with-zoom .main')

            allMain.select('.area').attr('d', mainArea)
            allMain.select('.x.axis').call(mainXAxis)

            brush.extent(window.brushExtent)
            brush(d3.selectAll('.brush'))
        }

        // Add current position line ==============
        main.append('line')
            .attr('class', 'chart-line-active')
            .attr('fill', 'red')
            .attr('x1', 0)
            .attr('y1', 0)
            .attr('x2', 0)
            .attr('y2', mainHeight)
            .attr('stroke', 'red')
            .attr("transform", `translate(${-mainMargin.left}, 0)`)

        const svgTarget = d3.selectAll('.chart-with-zoom svg'),
            lineCurrentTime = svgTarget.selectAll('.chart-line-active')

        svgTarget.on('mousemove', null)
        svgTarget.on('mousemove', function () {
            const coordinates = d3.mouse(this)

            lineCurrentTime
                .attr('x1', coordinates[0])
                .attr('x2', coordinates[0])
        })

        const focus = main.append('g')
            .style('display', 'none')
            .attr('class', 'focus-group')

        // Circle for hovered point
        focus.append('circle')
            .attr('class', 'y')
            .style('fill', 'none')
            .style('stroke', 'red')
            .attr('r', 4)

        // Tooltip for hovered point
        const tooltip = main.append('text')
            .attr('class', 'tooltip-current')
            .text('')

        // Rectangle for tracking mouse move
        svg.append('rect')
            .attr('width', W)
            .attr('height', mainHeight)
            .style('fill', 'none')
            .style('pointer-events', 'all')
            .attr('transform', `translate(${mainMargin.left}, ${mainMargin.top})`)
            .on('mouseover', () => d3.selectAll('.focus-group').style('display', null))
            .on('mouseout', () => d3.selectAll('.focus-group').style('display', 'none'))
            .on('mousemove', function () {
                const mouseCoord = d3.mouse(this)

                d3.selectAll('.main')[0].forEach(m => {
                    const mainContainer = d3.select(m),
                        localData = mainContainer.select('.area').data()[0]
                    // mainXScale = d3.time.scale().range([0, width]),
                    // mainYScale = d3.scale.linear().range([0, mainHeight])

                    if (!localData.length) return

                    mainXScale.domain(d3.extent(localData, d => d.t))
                    mainYScale.domain([0, d3.max(localData, d => d.v)])

                    subXScale.domain(mainXScale.domain())
                    subYScale.domain(mainYScale.domain())

                    mainXScale.domain(brush.empty() ? subXScale.domain() : brush.extent())

                    const x0 = mainXScale.invert(mouseCoord[0]),
                        i = bisectDate(localData, x0, 1),
                        d0 = localData[i - 1],
                        d1 = localData[i]

                    let coordX = 0,
                        coordY = 0,
                        d = { 't': 0, 'v': 0 }

                    if (d0 && d1) {
                        d = x0 - d0.t > d1.t - x0 ? d1 : d0
                        coordX = mainXScale(d.t)
                        coordY = mainYScale(d.v)
                    }

                    mainContainer.datum(localData)
                        .attr('d', mainArea)

                    mainContainer.select('.focus-group circle.y')
                        .attr('transform', `translate(${coordX}, ${coordY})`)

                    mainContainer.select('.tooltip-current')
                        .attr('x', `${coordX + 10}`)
                        .attr('y', '0')
                        .text(d.v)
                })
            })
    }

    render() {
        const title = this.props.title ? <h1>{this.props.title}</h1> : ''
        return (
            <div className="chart-with-zoom">
                {title}
                <svg ref="chartWithZoom"></svg>
            </div>
        )
    }
}

export default ChartD3Zoomed
