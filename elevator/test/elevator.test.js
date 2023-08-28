import { getLowestPrice } from '../src/price.finder.js';

test('canary', () => {
  expect(true).toBe(true);
});

test('getLowestPrice when no vendors are available', () => {
  expect(() => getLowestPrice(1, [])).toThrow('No vendors available');
});

test('getLowestPrice when number of days is negative', () => {
  expect(() => getLowestPrice(-1)).toThrow('Number of days must be positive'); 
});

test('getLowestPrice for one day when one vendor is available', () => {
  const vendor1PriceFinder = () => ({vendorName: 'Vendor1', price: 100});

  expect(getLowestPrice(1, [vendor1PriceFinder])).toStrictEqual({vendorNames: ['Vendor1'], dailyPrice: 100, totalPrice: 100}); 
});

test('getLowestPrice for three days when one vendor is available', () => {
  const vendor1PriceFinder = () => ({vendorName: 'Vendor1', price: 100});

  expect(getLowestPrice(3, [vendor1PriceFinder])).toStrictEqual({vendorNames: ['Vendor1'], dailyPrice: 100, totalPrice: 300}); 
});

test('getLowestPrice for one day when two vendors are available, first with better price', () => {
  const vendor1PriceFinder = () => ({vendorName: 'Vendor1', price: 50});

  const vendor2PriceFinder = () => ({vendorName: 'Vendor2', price: 100});

  expect(getLowestPrice(1, [vendor1PriceFinder, vendor2PriceFinder])).toStrictEqual({vendorNames: ['Vendor1'], dailyPrice: 50, totalPrice: 50});
});

test('getLowestPrice for one day when two vendors are available, second with better price', () => {
  const vendor1PriceFinder = () => ({vendorName: 'Vendor1', price: 50});

  const vendor2PriceFinder = () => ({vendorName: 'Vendor2', price: 25});

  expect(getLowestPrice(1, [vendor1PriceFinder, vendor2PriceFinder])).toStrictEqual({vendorNames: ['Vendor2'], dailyPrice: 25, totalPrice: 25});
});

test('getLowestPrice for one day when three vendors are available, first with better price', () => {
  const vendor1PriceFinder = () => ({vendorName: 'Vendor1', price: 25});

  const vendor2PriceFinder = () => ({vendorName: 'Vendor2', price: 40});

  const vendor3PriceFinder = () => ({vendorName: 'Vendor3', price: 30});

  expect(getLowestPrice(1, [vendor1PriceFinder, vendor2PriceFinder, vendor3PriceFinder])).toStrictEqual({vendorNames: ['Vendor1'], dailyPrice: 25, totalPrice: 25});
});

test('getLowestPrice for one day when three vendors are available, second with better price', () => {
  const vendor1PriceFinder = () => ({vendorName: 'Vendor1', price: 50});

  const vendor2PriceFinder = () => ({vendorName: 'Vendor2', price: 25});

  const vendor3PriceFinder = () => ({vendorName: 'Vendor3', price: 30});

  expect(getLowestPrice(1, [vendor1PriceFinder, vendor2PriceFinder, vendor3PriceFinder])).toStrictEqual({vendorNames: ['Vendor2'], dailyPrice: 25, totalPrice: 25});
});

test('getLowestPrice for one day when three vendors are available, third with better price', () => {
  const vendor1PriceFinder = () => ({vendorName: 'Vendor1', price: 50});

  const vendor2PriceFinder = () => ({vendorName: 'Vendor2', price: 30});

  const vendor3PriceFinder = () => ({vendorName: 'Vendor3', price: 25});

  expect(getLowestPrice(1, [vendor1PriceFinder, vendor2PriceFinder, vendor3PriceFinder])).toStrictEqual({vendorNames: ['Vendor3'], dailyPrice: 25, totalPrice: 25});
});

test('getLowestPrice for one day when three vendors are available, first one fails request, second has better price', () => {
  const vendor1PriceFinder = () => {throw new Error('Vendor1 is not available');};

  const vendor2PriceFinder = () => ({vendorName: 'Vendor2', price: 25});

  const vendor3PriceFinder = () => ({vendorName: 'Vendor3', price: 30});

  expect(getLowestPrice(1, [vendor1PriceFinder, vendor2PriceFinder, vendor3PriceFinder])).toStrictEqual({vendorNames: ['Vendor2'], dailyPrice: 25, totalPrice: 25});
});

test('getLowestPrice for one day when three vendors are available, all vendors failed to request', () => {
  const vendor1PriceFinder = () => {throw new Error('Vendor1 is not available');};

  const vendor2PriceFinder = () => {throw new Error('Vendor2 is not available');};

  const vendor3PriceFinder = () => {throw new Error('Vendor3 is not available');};

  expect(() => getLowestPrice(1, [vendor1PriceFinder, vendor2PriceFinder, vendor3PriceFinder])).toThrow('No vendors available'); 
});

test('getLowestPrice for one day when three vendors are available, the first and the third have the same lower price than the second', () => {
  const vendor1PriceFinder = () => ({vendorName: 'Vendor1', price: 25});

  const vendor2PriceFinder = () => ({vendorName: 'Vendor2', price: 30});

  const vendor3PriceFinder = () => ({vendorName: 'Vendor3', price: 25});

  expect(getLowestPrice(1, [vendor1PriceFinder, vendor2PriceFinder, vendor3PriceFinder])).toStrictEqual({vendorNames: ['Vendor1', 'Vendor3'], dailyPrice: 25, totalPrice: 25});
});