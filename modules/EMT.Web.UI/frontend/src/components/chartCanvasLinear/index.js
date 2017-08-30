class ChartCanvas extends React.Component {
    constructor(props) {
        super(props)

        this.updateChart = this.updateChart.bind(this)
    }

    shouldComponentUpdate() {
        return false
    }

    updateChart() {
        const maxPointsOnScreen = 500

        if (this.chart && this.props.data && this.props.data.length) {
            this.props.data.forEach(serie => {
                const existingSerie = this.chart.options.data && this.chart.options.data.filter(s => s.id === serie.id)[0]

                if (existingSerie) {
                    if(this.props.data.length === 1) {
                        existingSerie.color = '#7CB5EC'
                    }

                    existingSerie.type = this.props['chart-type']
                    existingSerie.dataPoints = existingSerie.dataPoints.concat(serie.data.map(el => ({
                        x: new Date(el[0]),
                        y: el[1]
                    })))
                    if (existingSerie.dataPoints.length > maxPointsOnScreen) {
                        existingSerie.dataPoints.splice(0, existingSerie.dataPoints.length - maxPointsOnScreen)
                    }
                } else {
                    console.log('Add new one')
                    // this.chart.addSeries(serie)
                }
            })
        }

        this.chart.render()
    }

    componentWillReceiveProps() {
        this.updateChart()
    }

    componentDidMount() {
        const data = this.props.data.map(serie => ({
            type: this.props['chart-type'],
            id: serie.id,
            dataPoints: serie.data.map(p => ({
                x: new Date(p[0]),
                y: p[1]
            }))
        }))

        this.chart = new CanvasJS.Chart(this.refs.chartContainer,
            {
                zoomEnabled: true,
                animationEnabled: true,
                title: {
                    text: this.props.title
                },
                axisX: {
                    labelAngle: -30
                },
                axisY: {
                    includeZero: false
                },
                data
            })

        this.updateChart()
    }

    render() {
        return <div ref="chartContainer" className="chart-canvas"></div>
    }
}

export default ChartCanvas
