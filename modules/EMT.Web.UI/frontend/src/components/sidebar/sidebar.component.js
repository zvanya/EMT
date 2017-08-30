import './sidebar.scss'

const convertDataToNestedTree = (orgData, counters) => {
    const formattedData = {}

    orgData.forEach(el => {
        el.path.split('.')
            .filter(id => id)
            .reduce((p, c, i, arrPath) => {
                if (arrPath.length - 1 === i) {
                    const additionalData = {}

                    // TODO: This is temporary solution ( Need API changes )
                    // "isLine" is an hardcoded property on API side
                    if (el.isLine) {
                        additionalData.counters = counters.filter(counter => counter.lineId === el.id)
                    }
                    p[c] = Object.assign({}, p[c], el, additionalData)
                } else {
                    if (!p[c] || !p[c].children) {
                        p[c] = Object.assign({}, p[c], { 'children': {} })
                    }
                }
                return p[c].children
            }, formattedData)
    })

    return formattedData
}

const sideBar = ({
    orgData,
    counters,
    selected_counters,
    onCounterClick
}) => {
    const buildNestedList = data => {
        return Object.keys(data).map((id, i) => {
            return (
                <ul key={i}>
                    <li>
                        <i className="fa fa-plus-circle" aria-hidden="true"></i>
                        {data[id].name}
                        {
                            data[id].counters && (
                                <ul>
                                    {
                                        data[id].counters.map((c, i) => (
                                            <li key={i}>
                                                <label>
                                                    <input
                                                        type="checkbox"
                                                        checked={selected_counters.indexOf(c.id) > -1}
                                                        onChange={() => onCounterClick(c.id)}
                                                    />
                                                    {c.name}
                                                </label>
                                            </li>
                                        ))
                                    }
                                </ul>
                            )
                        }
                    </li>
                    {
                        data[id].children && (
                            <li>
                                {buildNestedList(data[id].children)}
                            </li>
                        )
                    }
                </ul>
            )
        })
    }

    return (
        <div className="sidebar height-all">
            { buildNestedList(convertDataToNestedTree(orgData, counters)) }
        </div>
    )
}

export default sideBar
