export const getRandomArbitrary = (min, max) => Math.random() * (max - min) + min

export const generateDummyData = elNumber => {
    let data = [],
        time = (new Date()).getTime()

    for (let i = 0, l = elNumber; i < l; i++) {
        data.push([
            time + i * 1000,
            Math.round(Math.random() * 100)
        ]);
    }
    return data
}

export const getDateInDaysFromToday = (days = 0) => {
    const d = new Date()
    d.setDate(d.getDate() + days)

    return d.getTime()
}
