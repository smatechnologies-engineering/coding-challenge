// import RequestMap from '../elevator/requestMap.js'
import RequestMap from '../requestMap.js'

describe('RequestMap', () => {
  let requestMap: RequestMap<number>;
  beforeEach(() => {
    // Create a new Elevator instance before each test
    requestMap = new RequestMap<number>();
  });

  it('both max and min should be -1 if there requestMap.map size = 0', () => {
    expect(requestMap.map.size).toBe(0);
    expect(requestMap.min).toBe(-1)
    expect(requestMap.max).toBe(-1)
  });

  it('both max and min should be -1 if item is removed to make size = 0', () => {
    requestMap.set(1,1);
    requestMap.delete(1);
    expect(requestMap.min).toBe(-1)
    expect(requestMap.max).toBe(-1)
  });

  it('should correctly set max/min when a higher number is added', () => {
    requestMap.set(1,1);
    requestMap.set(2,1);

    expect(requestMap.max).toEqual(2);
    expect(requestMap.min).toEqual(1);

  })

  it('should correctly set min/min when a lower number is added', () => {
    requestMap.set(4,1);
    requestMap.set(3,1);

    expect(requestMap.min).toEqual(3);
    expect(requestMap.max).toEqual(4);

  })

  it('min should be reset to correct number if current min is deleted and size !== 0', () => {
    requestMap.set(1,1);
    requestMap.set(2,1)
    requestMap.delete(1);
    expect(requestMap.min).toBe(2)
    expect(requestMap.max).toBe(2)
  });

  it('max should be reset to correct number if current max is removed and size !== 0', () => {
    requestMap.set(1,1);
    requestMap.set(2,1)
    requestMap.delete(2);
    expect(requestMap.min).toBe(1)
    expect(requestMap.max).toBe(1)
  });




});