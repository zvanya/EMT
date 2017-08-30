import ChartD3 from '../../components/chartD3/'
import ChartD3Zoomed from '../../components/chartD3Zoomed/'

class DashboardPage extends React.Component {
    render() {
        let { data, selected_counters, counters, dateTo, dateFrom } = this.props

        return (
            <div>
                {
                    selected_counters.map((scId, i) => {
                        if (data[scId] && data[scId].length) {
                            return (
                                <div key={i}>
                                    <h1>{counters.filter(c => c.id === scId)[0].name}</h1>
                                    <ChartD3Zoomed data={data[scId]} dateFrom={dateFrom} dateTo={dateTo} />
                                </div>
                            )
                        } else if (data[scId] && !data[scId].length) {
                            return (
                                <div key={i} className="chart-loader">
                                    <span>
                                        <i className="fa fa-exclamation-triangle" aria-hidden="true"></i>
                                        <span className="msg-no-data"> Sorry, but there are no data for counter "{counters.filter(c => c.id === scId)[0].name}" for selected period.</span>
                                    </span>
                                </div>
                            )
                        } else {
                            return (
                                <div key={i} className="chart-loader">
                                    <i className="fa fa-spinner fa-spin fa-3x fa-fw"></i>
                                    <span className="sr-only">Loading...</span>
                                </div>
                            )
                        }
                    })
                }
            </div>
        )
    }
}

export default DashboardPage
